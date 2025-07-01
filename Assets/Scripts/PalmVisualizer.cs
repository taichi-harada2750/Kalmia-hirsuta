using UnityEngine;

public class PalmVisualizer : MonoBehaviour
{
    public enum HandType { Left, Right }
    public HandType handType = HandType.Right;

    void Update()
    {
        Vector3 pos = handType == HandType.Right ? PalmDataManager.RightPalm : PalmDataManager.LeftPalm;
        transform.position = pos;
    }
}
