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

        void Start()
        {
            col3d = GetComponent<Collider>();
            // サイズを質量に比例させる
            transform.localScale = Vector3.one * Mathf.Sqrt(mass);
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
                    // 捕食される
                    StartCoroutine(AbsorbRoutine(slime));
                }
                else
                {
                    // 逆に食われる（ダメージを受ける）
                    slime.AddMass(-this.mass);
                    // ターゲット側も消滅するか、弾かれるか
                    // ここではダメージを与えて自身は消滅する仕様とする
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
            
            // スコア加算 (DualAimShootingのAPI流用)
            if (SortGameManager.Instance != null)
            {
                SortGameManager.Instance.AddScore(true); // trueは通常加算など、プロジェクトの仕様に合わせて
            }

            Destroy(gameObject);
        }
    }
}
