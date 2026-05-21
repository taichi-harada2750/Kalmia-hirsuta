using UnityEngine;
using System.Collections;

namespace FluidSlime
{
    public class SlimeTarget : MonoBehaviour
    {
        public float mass = 0.5f;
        public float absorbDuration = 0.5f;

        private bool isBeingAbsorbed = false;
        private Collider col3d;
        private Renderer meshRenderer;

        public float sizeMultiplier = 30.0f;
        
        [Header("AI Movement")]
        public float moveSpeed = 15f; // 逃げ切れるように速度を下げる
        private Vector3 spawnCenter = new Vector3(5.1f, 65f, 27.9f); // 画面中央
        
        public float lifeTime = 12f; // 12秒で自然消滅（画面に溜まり続けるのを防ぐ）
        private float lifeTimer = 0f;

        void Start()
        {
            col3d = GetComponent<Collider>();
            meshRenderer = GetComponent<Renderer>();
            
            // スライム本体と同じ対数成長カーブを使用
            float scaleFactor = Mathf.Log10((mass * 9f) + 1f);
            transform.localScale = Vector3.one * scaleFactor * sizeMultiplier;
        }

        void Update()
        {
            if (isBeingAbsorbed || meshRenderer == null || SlimeController.Instance == null) return;

            // 自然消滅ロジック
            lifeTimer += Time.deltaTime;
            if (lifeTimer >= lifeTime)
            {
                Destroy(gameObject);
                return;
            }

            bool isEdible = SlimeController.Instance.totalMass >= this.mass;

            // 自分の質量とスライムの質量を比較し、Standardマテリアルの色を変更
            // 緑は避けて、安全はシアン系(青水色)、危険は赤とする
            if (isEdible)
            {
                meshRenderer.material.color = new Color(0.2f, 0.8f, 1.0f); // 食べられる色（シアン系）
            }
            else
            {
                meshRenderer.material.color = Color.red; // 危険な色
            }

            // AI移動ロジック
            if (FluidSlimeApp.Instance != null && FluidSlimeApp.Instance.IsGameRunning)
            {
                Vector3 slimeCenter = (SlimeController.Instance.coreA.transform.position + SlimeController.Instance.coreB.transform.position) / 2f;
                Vector3 dirToSlime = (slimeCenter - transform.position).normalized;
                
                // Z軸は固定して2D平面移動にする
                dirToSlime.z = 0;
                
                Vector3 moveDir = Vector3.zero;

                if (isEdible)
                {
                    // 逃げる
                    moveDir = -dirToSlime;
                }
                else
                {
                    // 追ってくる
                    moveDir = dirToSlime;
                }

                // 画面端への逃げ込み防止（中央への緩やかな引力）
                Vector3 dirToCenter = (spawnCenter - transform.position).normalized;
                dirToCenter.z = 0;
                
                // 中心から離れるほど中央への引力が強くなる
                float distFromCenter = Vector2.Distance(spawnCenter, transform.position);
                float centerPullForce = Mathf.Clamp01(distFromCenter / 200f); // 200以上離れると強く引き戻す

                // 追尾と引力を合成
                Vector3 finalVelocity = Vector3.Lerp(moveDir, dirToCenter, centerPullForce * 0.8f).normalized * moveSpeed;
                
                // スケールが大きい（質量が大きい）と少し遅くなるようにする（巨大敵はゆっくり動く）
                float speedModifier = Mathf.Clamp(1f / Mathf.Sqrt(mass), 0.3f, 1.5f);
                
                transform.position += finalVelocity * speedModifier * Time.deltaTime;
            }
        }

        void OnTriggerEnter(Collider other)
        {
            if (isBeingAbsorbed) return;

            // スライムのコライダーに触れたか判定
            SlimeController slime = other.GetComponentInParent<SlimeController>();
            if (slime != null)
            {
                if (slime.totalMass >= this.mass)
                {
                    // 捕食可能
                    StartCoroutine(AbsorbRoutine(slime));
                }
                else
                {
                    // 自分が大きいのでプレイヤーを捕食する（一撃でゲームオーバー）
                    slime.AddMass(-9999f);
                    Destroy(gameObject);
                }
            }
        }

        IEnumerator AbsorbRoutine(SlimeController slime)
        {
            isBeingAbsorbed = true;
            if (col3d != null) col3d.enabled = false;

            Vector3 startPos = transform.position;
            Vector3 startScale = transform.localScale;
            float elapsed = 0f;

            while (elapsed < absorbDuration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / absorbDuration;
                
                // Ease In
                t = t * t;

                // スライムの中心（2つのコアの中間）へ向かって吸い込まれる
                Vector3 slimeCenter = (slime.coreA.transform.position + slime.coreB.transform.position) / 2f;
                transform.position = Vector3.Lerp(startPos, slimeCenter, t);
                
                // 縮小
                transform.localScale = Vector3.Lerp(startScale, Vector3.zero, t);

                yield return null;
            }

            // 吸収完了
            slime.AddMass(this.mass);
            
            // スコア加算（サイズに応じた加算）
            if (SortGameManager.Instance != null)
            {
                int scoreToAdd = Mathf.Max(1, Mathf.FloorToInt(this.mass / 2f));
                // ループでAddScoreを何度も呼ぶと、UIの拡大アニメーションや効果音が重複して爆発するため、
                // scoreの数値を直接加算し、イベント発火（音や演出）は1回だけ行うようにします。
                if (scoreToAdd > 1)
                {
                    SortGameManager.Instance.score += (scoreToAdd - 1);
                }
                SortGameManager.Instance.AddScore(true); 
            }

            // タイム延長
            if (FluidSlimeApp.Instance != null)
            {
                FluidSlimeApp.Instance.ExtendTimer(1.0f);
            }

            Destroy(gameObject);
        }
    }
}
