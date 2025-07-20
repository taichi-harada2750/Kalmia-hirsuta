using UnityEngine;

public class PalmVisualizerold : MonoBehaviour
{
    public enum HandType { Left, Right }
    public HandType handType = HandType.Right;

    public float scale = 50f;
    private float initialZ;

    void Start()
    {
        // 初期Z座標（高さ）を保持
        initialZ = transform.position.z;
    }

    void Update()
    {
        Vector3 sourcePos = handType == HandType.Right
            ? PalmDataManager.RightPalm
            : PalmDataManager.LeftPalm;

        // XとYをそのまま、Zを固定
        Vector3 converted = new Vector3(
            sourcePos.x * scale,
            sourcePos.y * scale,
            initialZ                  // Z軸（高さ）は固定
        );

        transform.position = converted;
    }
}
