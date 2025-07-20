using UnityEngine;

public class HandGrabber : MonoBehaviour
{
    public enum HandType { Left, Right }
    public HandType handType = HandType.Right;

    private GrabbableObject heldObject = null;
    private bool isGrabbing = false;

    void Update()
    {
        // 手の位置をPalmDataManagerから取得
        Vector3 palmPosition = handType == HandType.Right ? PalmDataManager.RightPalm : PalmDataManager.LeftPalm;
        isGrabbing = handType == HandType.Right ? PalmDataManager.RightGrabbing : PalmDataManager.LeftGrabbing;

        transform.position = palmPosition;

        // 掴んでない状態で離したら解放
        if (!isGrabbing && heldObject != null)
        {
            heldObject.Release();
            heldObject = null;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (heldObject == null && isGrabbing)
        {
            GrabbableObject go = other.GetComponent<GrabbableObject>();
            if (go != null && !go.IsBeingHeld)
            {
                heldObject = go;
                heldObject.Grab(this.transform);
            }
        }
    }
}
