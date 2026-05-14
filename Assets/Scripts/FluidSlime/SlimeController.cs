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
        public float splitThresholdMultiplier = 4.0f; // radius * this = split distance
        public float mergeThresholdMultiplier = 2.5f; // radius * this = merge distance
        public float snapForce = 500f; // 分裂時の反発力
        
        [Header("Colliders")]
        public SphereCollider colA;
        public SphereCollider colB;
        public BoxCollider bridgeCol;

        public bool isMerged { get; private set; } = true;
        
        [Header("Status")]
        public float totalMass = 2.0f; 
        public float currentTension = 0f;

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

        void UpdateCoresMass()
        {
            if (coreA != null) coreA.currentMass = totalMass / 2f;
            if (coreB != null) coreB.currentMass = totalMass / 2f;
        }

        void Update()
        {
            if (coreA == null || coreB == null) return;

            // MediaPipeからの入力（テスト時はマウスなどにする処理が別途App側にある想定）
            coreA.SetTargetPosition(PalmDataManager.LeftPalm);
            coreB.SetTargetPosition(PalmDataManager.RightPalm);

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
            if (colA != null)
            {
                colA.transform.position = coreA.transform.position;
                colA.radius = coreA.Radius;
            }
            if (colB != null)
            {
                colB.transform.position = coreB.transform.position;
                colB.radius = coreB.Radius;
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
                    float thickness = avgRadius * 2f * (1f - tension * 0.5f); // 引っ張ると最大50%細くなる
                    bridgeCol.size = new Vector3(dist, thickness, thickness);
                }
                else
                {
                    bridgeCol.enabled = false;
                }
            }
        }
    }
}
