using UnityEngine;

namespace FluidSlime
{
    public class SlimeController : MonoBehaviour
    {
        [Header("Cores")]
        public SlimeCore coreA; // Left Hand
        public SlimeCore coreB; // Right Hand
        
        [Header("Rendering")]
        public Material membraneMaterial;
        
        [Header("Hysteresis Settings")]
        public float splitThresholdMultiplier = 100.0f; // radius * this = split distance
        public float mergeThresholdMultiplier = 90.0f; // radius * this = merge distance
        public float snapForce = 500f; // 分裂時の反発力
        
        [Header("Colliders")]
        public SphereCollider colA;
        public SphereCollider colB;
        public BoxCollider bridgeCol;

        public bool isMerged { get; private set; } = true;
        
        [Header("Status")]
        public float totalMass = 2.0f; 
        public float currentTension = 0f;

        [Header("Collider Tuning")]
        [Tooltip("実際の描画サイズに対する当たり判定の割合（誤爆防止のため小さめに設定）")]
        public float colliderScaleMultiplier = 0.6f;

        // シングルトン的にアクセスしやすくするため
        public static SlimeController Instance { get; private set; }

        void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);
        }

        void Start()
        {
            UpdateCoresMass();
        }

        public void AddMass(float amount)
        {
            totalMass += amount;
            // 質量が0以下になったらゲームオーバーなどの処理も可能
            if (totalMass < 0.5f) totalMass = 0.5f; 
            UpdateCoresMass();
        }

        public void ResetSlime()
        {
            totalMass = 2.0f;
            isMerged = true;
            UpdateCoresMass();
            
            if (coreA != null) coreA.ResetPhysics();
            if (coreB != null) coreB.ResetPhysics();
        }

        void UpdateCoresMass()
        {
            if (coreA != null) coreA.currentMass = totalMass / 2f;
            if (coreB != null) coreB.currentMass = totalMass / 2f;
        }

        void Update()
        {
            if (coreA == null || coreB == null) return;

            // MediaPipeからの入力
            Vector3 targetA = PalmDataManager.LeftPalm;
            Vector3 targetB = PalmDataManager.RightPalm;

            // MediaPipeの入力がない（(0,0,0)のまま）場合はマウス位置でテストできるようにする
            if (targetA == Vector3.zero && targetB == Vector3.zero)
            {
                if (Camera.main != null)
                {
                    Vector3 mousePos = Input.mousePosition;
                    mousePos.z = 27.9f - Camera.main.transform.position.z;
                    Vector3 worldMouse = Camera.main.ScreenToWorldPoint(mousePos);
                    
                    targetA = worldMouse + Vector3.left * 20f;
                    targetB = worldMouse + Vector3.right * 20f;
                }
            }
            else
            {
                // MediaPipe入力の場合、Z座標をゲーム空間の基準面（27.9f）に固定する
                // これを行わないと、見た目（シェーダーは2D）は重なっていても、3D空間上ではZ軸がズレていてコライダーが接触しなくなります
                targetA.z = 27.9f;
                targetB.z = 27.9f;
            }

            coreA.SetTargetPosition(targetA);
            coreB.SetTargetPosition(targetB);

            float dist = Vector3.Distance(coreA.transform.position, coreB.transform.position);
            float currentRadiusA = coreA.Radius;
            float currentRadiusB = coreB.Radius;
            float avgRadius = (currentRadiusA + currentRadiusB) / 2f;
            
            // Hysteresis logic
            if (isMerged)
            {
                if (dist > avgRadius * splitThresholdMultiplier)
                {
                    isMerged = false;
                    // 分裂時のスナップ反発力（互いに遠ざかる方向へ）
                    Vector3 dir = (coreB.transform.position - coreA.transform.position).normalized;
                    coreA.AddForce(-dir * snapForce);
                    coreB.AddForce(dir * snapForce);
                }
            }
            else
            {
                if (dist < avgRadius * mergeThresholdMultiplier)
                {
                    isMerged = true;
                }
            }
            
            // Tension Calculation (0 to 1) when merged
            currentTension = 0f;
            if (isMerged)
            {
                float minD = avgRadius * mergeThresholdMultiplier;
                float maxD = avgRadius * splitThresholdMultiplier;
                currentTension = Mathf.Clamp01((dist - minD) / (maxD - minD));

                // 代謝（テンションが高い＝手を広げている ほど質量を消費する）
                if (currentTension > 0.1f && FluidSlimeApp.Instance != null && FluidSlimeApp.Instance.IsGameRunning)
                {
                    // テンションMAXで毎秒0.5の質量を失うリスク
                    AddMass(-currentTension * 0.5f * Time.deltaTime);
                }
            }
            
            // Update Core Physics
            coreA.UpdatePhysicsParams(currentTension);
            coreB.UpdatePhysicsParams(currentTension);
            
            // Update Shader
            if (membraneMaterial != null)
            {
                membraneMaterial.SetVector("_CoreA", coreA.transform.position);
                membraneMaterial.SetVector("_CoreB", coreB.transform.position);
                membraneMaterial.SetFloat("_RadiusA", currentRadiusA);
                membraneMaterial.SetFloat("_RadiusB", currentRadiusB);
                
                // 分裂している場合は _SminFactor を 0に近づけて膜を切る
                float targetSmin = isMerged ? 1.5f * avgRadius : 0.01f;
                float currentSmin = membraneMaterial.GetFloat("_SminFactor");
                membraneMaterial.SetFloat("_SminFactor", Mathf.Lerp(currentSmin, targetSmin, Time.deltaTime * 10f));
            }
            
            UpdateColliders(dist, currentTension);
        }

        void UpdateColliders(float dist, float tension)
        {
            // Shaderのsmin（滑らかな結合）によって、見た目の境界線は実際のRadiusより sminFactor/4 ほど外側に膨張します
            float currentSmin = membraneMaterial != null ? membraneMaterial.GetFloat("_SminFactor") : 0f;
            float sminBloat = currentSmin / 4f;

            if (colA != null)
            {
                colA.transform.position = coreA.transform.position;
                float scaleA = colA.transform.lossyScale.x != 0 ? Mathf.Abs(colA.transform.lossyScale.x) : 1f;
                colA.radius = ((coreA.Radius + sminBloat) * colliderScaleMultiplier) / scaleA;
            }
            if (colB != null)
            {
                colB.transform.position = coreB.transform.position;
                float scaleB = colB.transform.lossyScale.x != 0 ? Mathf.Abs(colB.transform.lossyScale.x) : 1f;
                colB.radius = ((coreB.Radius + sminBloat) * colliderScaleMultiplier) / scaleB;
            }

            if (bridgeCol != null)
            {
                if (isMerged)
                {
                    bridgeCol.enabled = true;
                    // 中間点
                    Vector3 midPoint = (coreA.transform.position + coreB.transform.position) / 2f;
                    bridgeCol.transform.position = midPoint;
                    
                    // 角度
                    Vector3 dir = coreB.transform.position - coreA.transform.position;
                    if (dir != Vector3.zero)
                    {
                        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                        bridgeCol.transform.rotation = Quaternion.Euler(0, 0, angle);
                    }
                    
                    // サイズ（テンションが高いと細くなる）
                    float avgRadius = (coreA.Radius + coreB.Radius) / 2f;
                    float visualThickness = (avgRadius + sminBloat) * 2f;
                    float thickness = visualThickness * (1f - tension * 0.5f) * colliderScaleMultiplier; 
                    
                    // 距離(X軸)も少し短くしてコア判定からのはみ出しを防ぐ
                    float xLength = Mathf.Max(0, dist - ((avgRadius + sminBloat) * colliderScaleMultiplier));

                    // Transformのスケールが1以外の場合の補正（巨大化バグ防止）
                    float scaleX = bridgeCol.transform.lossyScale.x != 0 ? Mathf.Abs(bridgeCol.transform.lossyScale.x) : 1f;
                    float scaleY = bridgeCol.transform.lossyScale.y != 0 ? Mathf.Abs(bridgeCol.transform.lossyScale.y) : 1f;
                    float scaleZ = bridgeCol.transform.lossyScale.z != 0 ? Mathf.Abs(bridgeCol.transform.lossyScale.z) : 1f;

                    bridgeCol.size = new Vector3(xLength / scaleX, thickness / scaleY, thickness / scaleZ);
                }
                else
                {
                    bridgeCol.enabled = false;
                }
            }
        }
    }
}
