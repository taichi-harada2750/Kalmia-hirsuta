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
