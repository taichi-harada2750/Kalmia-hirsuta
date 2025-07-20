using UnityEngine;

public class PalmVisualizer_NoSprite : MonoBehaviour
{
    public enum HandType { Left, Right }
    public HandType handType = HandType.Right;

    public float scale = 50f;
    public float zOffset = 0f;

    public Vector3 debugOffset = Vector3.zero;

    void Update()
    {
        Vector3 palm = handType == HandType.Right
            ? PalmDataManager.RightPalm
            : PalmDataManager.LeftPalm;

        Vector3 pos = new Vector3(
            palm.x * scale,
            palm.y * scale,
            zOffset
        );

        transform.position = pos + debugOffset;
    }
}
