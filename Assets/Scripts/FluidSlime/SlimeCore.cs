using UnityEngine;

namespace FluidSlime
{
    public class SlimeCore : MonoBehaviour
    {
        [Header("Spring Physics")]
        public float springConstant = 250f;
        public float damping = 15f;
        
        [Header("Mass & Size")]
        public float currentMass = 1.0f;
        public float baseRadius = 30.0f;

        private Vector3 velocity = Vector3.zero;
        private Vector3 targetPosition;

        // 手動で力を加えるための変数（分裂時のスナップなど）
        private Vector3 externalForce = Vector3.zero;

        // 質量が増えてもインフレしすぎないように対数的な成長カーブに変更
        // Mathf.Log10(currentMass * 9 + 1) は、mass=1で1倍、mass=10で2倍、mass=100で3倍になる
        public float Radius => baseRadius * Mathf.Log10((currentMass * 9f) + 1f);

        void Start()
        {
            targetPosition = transform.position;
        }

        public void SetTargetPosition(Vector3 newTarget)
        {
            targetPosition = newTarget;
        }

        public void AddForce(Vector3 force)
        {
            externalForce += force;
        }

        public void ResetPhysics()
        {
            velocity = Vector3.zero;
            externalForce = Vector3.zero;
        }

        // テンションに応じてバネ定数やダンピングを変える機能（オプション）
        public void UpdatePhysicsParams(float tensionModifier)
        {
            // tensionModifier が高い（伸びている）ほど動きを重くするか、逆に軽くするか
            // ここでは張力が高いと動きが重くなる（ダンピング増加）と仮定
            damping = 15f + (tensionModifier * 10f);
        }

        void Update()
        {
            // フックの法則: F = -k * x
            Vector3 displacement = transform.position - targetPosition;
            Vector3 springForce = -springConstant * displacement;

            // F = ma -> a = F/m
            // ここでスライムの質量(currentMass)が大きいほど加速度が鈍る（慣性が大きくなる）
            Vector3 acceleration = (springForce + externalForce) / currentMass;
            
            // ダンピング（空気抵抗・内部摩擦）
            acceleration -= velocity * damping;

            velocity += acceleration * Time.deltaTime;
            transform.position += velocity * Time.deltaTime;

            // 加えた外力は1フレームでリセット（Impulse的な扱い）
            externalForce = Vector3.zero;
        }
    }
}
