using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class PalmVisualizer : MonoBehaviour
{
    public enum HandType { Left, Right }
    public HandType handType = HandType.Right;

    public Sprite normalSprite;     // 通常状態のカーソル画像
    public Sprite grabbingSprite;   // Grab状態のカーソル画像

    private SpriteRenderer spriteRenderer;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        // 物理検知漏れ（特に左手など）を防ぐため、コライダーとRigidbodyがなければ自動追加する
        if (GetComponent<Collider>() == null)
        {
            var col = gameObject.AddComponent<SphereCollider>();
            col.radius = 0.5f; // デフォルトの適切なサイズ
            col.isTrigger = true;
            Debug.LogWarning($"[{gameObject.name}] コライダーがアタッチされていなかったため、自動追加しました。");
        }
        if (GetComponent<Rigidbody>() == null)
        {
            var rb = gameObject.AddComponent<Rigidbody>();
            rb.isKinematic = true;
            rb.useGravity = false;
        }
    }

    void Update()
    {
        // 手の位置に追従
        Vector3 pos = handType == HandType.Right ? PalmDataManager.RightPalm : PalmDataManager.LeftPalm;
        transform.position = pos;

        // Grab状態に応じてSpriteを切り替え
        bool isGrabbing = handType == HandType.Right ? PalmDataManager.RightGrabbing : PalmDataManager.LeftGrabbing;
        spriteRenderer.sprite = isGrabbing ? grabbingSprite : normalSprite;
    }
}
