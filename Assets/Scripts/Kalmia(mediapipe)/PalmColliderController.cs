using UnityEngine;

public class PalmColliderController : MonoBehaviour
{
    public enum HandType { Left, Right }
    public HandType handType = HandType.Right;
    private Collider col;

    void Start()
    {
        col = GetComponent<Collider>();
        col.enabled = false; // 最初はオフ
    }

    void Update()
    {
        bool isGrabbing = handType == HandType.Right ? PalmDataManager.RightGrabbing : PalmDataManager.LeftGrabbing;
        col.enabled = isGrabbing;
    }
}
