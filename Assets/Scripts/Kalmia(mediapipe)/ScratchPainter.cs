using UnityEngine;

public class ScratchPainter : MonoBehaviour
{
    [Header("描画設定")]
    [SerializeField] private RenderTexture maskRT;
    [SerializeField] private Texture2D brushTexture;
    [SerializeField] private float brushSize = 0.01f;

    [Header("レイヤー設定")]
    [SerializeField] private LayerMask scratchLayer;   // OnCollision用（球の当たり判定）
    [SerializeField] private LayerMask raycastLayer;   // Raycast用（UV取得対象）

    void Start()
    {
        Collider col = GetComponent<Collider>();
        if (col == null)
        {
            Debug.LogError($"[ScratchPainter] コライダーがありません！");
        }
        else
        {
            Debug.Log($"[ScratchPainter] コライダーあり: {col.GetType().Name}, enabled: {col.enabled}, trigger: {col.isTrigger}");
        }

        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogWarning("[ScratchPainter] Rigidbodyがありません（Kinematic推奨）");
        }
        else
        {
            Debug.Log($"[ScratchPainter] Rigidbody設定: isKinematic={rb.isKinematic}, useGravity={rb.useGravity}");
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        Debug.Log($"[ScratchPainter] 衝突元: {gameObject.name}, 衝突相手: {collision.gameObject.name}");
        

        foreach (ContactPoint contact in collision.contacts)
        {
            Debug.Log($"[ScratchPainter] Contact: {contact.point}, Layer: {contact.otherCollider.gameObject.layer}");

            if (((1 << contact.otherCollider.gameObject.layer) & scratchLayer) == 0)
            {
                Debug.Log($"[ScratchPainter] 無視されたレイヤー: {contact.otherCollider.gameObject.layer}");
                continue;
            }

            Debug.Log($"[ScratchPainter] Scratch対象と判定 → 試しにUV取得します");

            if (TryGetUV(contact.point, out Vector2 uv, contact))
            {
                Debug.Log($"[ScratchPainter] UVヒット: {uv}");
                DrawAtUV(uv);
            }
            else
            {
                Debug.LogWarning($"[ScratchPainter] UV取得失敗: {contact.point}");
            }
        }
    }


    bool TryGetUV(Vector3 worldPos, out Vector2 uv, ContactPoint contact)
    {
        uv = Vector2.zero;

        // カーソルのZ+方向にRayを飛ばす
        Ray ray = new Ray(worldPos - transform.forward * 0.01f, transform.forward);
        Debug.DrawRay(ray.origin, ray.direction * 10f, Color.cyan, 2f);

        if (Physics.Raycast(ray, out RaycastHit hit, 10f, raycastLayer))
        {
            Bounds bounds = hit.collider.bounds;
            Vector3 localPoint = hit.point - bounds.min;
            Vector3 size = bounds.size;

            float u = 1f - (localPoint.x / size.x);  // 水平方向（左右）  ← flip必要ならここは調整
            float v = localPoint.y / size.y;         // 垂直方向（上下）  ← Y基準でOK（壁面想定）

            uv = new Vector2(u, v);
            Debug.Log($"[ScratchPainter] Ray HIT: {hit.collider.name}, Point: {hit.point}, UV: {uv}");
            return true;
        }

        Debug.LogWarning($"[ScratchPainter] RAY FAILED at {ray.origin}, dir: {ray.direction}, layer={raycastLayer.value}");
        return false;
    }



    void DrawAtUV(Vector2 uv)
    {
        Debug.Log($"[ScratchPainter] 描画開始: UV={uv}");

        if (maskRT == null || brushTexture == null)
        {
            Debug.LogError("[ScratchPainter] RenderTexture または BrushTexture が未設定です！");
            return;
        }

        Vector2 pixel = new Vector2(uv.x * maskRT.width, uv.y * maskRT.height);

        RenderTexture.active = maskRT;
        GL.PushMatrix();
        GL.LoadPixelMatrix(0, maskRT.width, maskRT.height, 0);

        Graphics.DrawTexture(
            new Rect(pixel.x - brushTexture.width / 2, pixel.y - brushTexture.height / 2,
                     brushTexture.width, brushTexture.height),
            brushTexture);

        GL.PopMatrix();
        RenderTexture.active = null;

        Debug.Log($"[ScratchPainter] ピクセル描画完了: {pixel}");
    }

    public void ClearMask()
    {
        if (maskRT == null)
        {
            Debug.LogWarning("[ScratchPainter] ClearMask: maskRT が null です");
            return;
        }

        RenderTexture.active = maskRT;
        GL.Clear(true, true, Color.black); // マスクを黒でクリア（完全削除）
        RenderTexture.active = null;

        Debug.Log("[ScratchPainter] マスクをクリアしました");
    }
    private void FixedUpdate()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, 0.05f);
        Debug.Log($"[ScratchPainter] 衝突数: {hits.Length}");

        foreach (var hit in hits)
        {
            Debug.Log($"[ScratchPainter] 衝突検知: {hit.name} | Layer: {hit.gameObject.layer}");
        }
    }

}

