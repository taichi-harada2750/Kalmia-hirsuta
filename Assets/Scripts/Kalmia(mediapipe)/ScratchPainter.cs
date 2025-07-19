using UnityEngine;

public class ScratchPainter : MonoBehaviour
{
    [Header("描画設定")]
    [SerializeField] private RenderTexture maskRT;
    [SerializeField] private Texture2D brushTexture;
    [SerializeField] private float brushSize = 0.01f;

    [Header("レイヤー設定")]
    [SerializeField] private LayerMask scratchLayer;   // 接触対象
    [SerializeField] private LayerMask raycastLayer;   // RaycastでUV取得対象

    private void OnCollisionStay(Collision collision)
    {
        foreach (ContactPoint contact in collision.contacts)
        {
            // 指定レイヤー以外は無視
            if (((1 << contact.otherCollider.gameObject.layer) & scratchLayer) == 0)
                continue;

            if (TryGetUV(contact.point, out Vector2 uv, contact))
            {
                DrawAtUV(uv);
            }
        }
    }

    bool TryGetUV(Vector3 worldPos, out Vector2 uv, ContactPoint contact)
    {
        uv = Vector2.zero;

        Ray ray = new Ray(worldPos + Vector3.up * 0.01f, Vector3.down);
        if (Physics.Raycast(ray, out RaycastHit hit, 10f, raycastLayer))
        {
            Bounds bounds = hit.collider.bounds;
            Vector3 localPoint = hit.point - bounds.min;
            Vector3 size = bounds.size;

            float u = 1f - (localPoint.x / size.x);
            float v = localPoint.z / size.z;
            uv = new Vector2(u, v);

            return true;
        }

        return false;
    }

    void DrawAtUV(Vector2 uv)
    {
        if (maskRT == null || brushTexture == null) return;

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
    }

    public void ClearMask()
    {
        if (maskRT == null) return;

        RenderTexture.active = maskRT;
        GL.Clear(true, true, Color.black);
        RenderTexture.active = null;
    }
}
