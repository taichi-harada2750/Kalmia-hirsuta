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

    private void OnCollisionStay(Collision collision)
    {
        foreach (ContactPoint contact in collision.contacts)
        {
            // スクラッチ対象のレイヤーにのみ反応
            if (((1 << contact.otherCollider.gameObject.layer) & scratchLayer) == 0)
                continue;

            if (TryGetUV(contact.point, out Vector2 uv, contact))
            {
                Debug.Log($"[ScratchPainter] UV hit: {uv}");
                DrawAtUV(uv);
            }
            else
            {
                Debug.LogWarning($"[ScratchPainter] UV acquisition failed at {contact.point}");
            }
        }
    }

    bool TryGetUV(Vector3 worldPos, out Vector2 uv, ContactPoint contact)
    {
        uv = Vector2.zero;

        // XY平面用のRay（Z方向からPlaneに向かって打つ）
        Ray ray = new Ray(worldPos + Vector3.back * 0.01f, Vector3.forward);
        Debug.DrawRay(ray.origin, ray.direction * 10f, Color.red, 2f);

        if (Physics.Raycast(ray, out RaycastHit hit, 10f, raycastLayer))
        {
            Bounds bounds = hit.collider.bounds;
            Vector3 localPoint = hit.point - bounds.min;
            Vector3 size = bounds.size;

            // XY平面：Xを横軸、Yを縦軸としてUVを構成
            float u = localPoint.x / size.x;
            float v = localPoint.y / size.y;
            uv = new Vector2(u, v);

            Debug.Log($"[ScratchPainter] Ray HIT {hit.collider.name}, point: {hit.point}, UV: {uv}");
            return true;
        }

        Debug.LogWarning($"[ScratchPainter] RAY FAILED at {ray.origin}, dir: {ray.direction}, layer={raycastLayer.value}");
        return false;
    }











    void DrawAtUV(Vector2 uv)
    {
        Debug.Log($"Drawing at {uv}");
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

        Debug.Log($"[ScratchPainter] Drawn at pixel: {pixel}");
    }
    public void ClearMask()
    {
        if (maskRT == null)
        {
            Debug.LogWarning("[ScratchPainter] ClearMask: maskRT is null.");
            return;
        }

        RenderTexture.active = maskRT;
        GL.Clear(true, true, Color.black); // マスクを完全に消す（透明化）
        RenderTexture.active = null;

        Debug.Log("[ScratchPainter] Mask cleared.");
    }

}

