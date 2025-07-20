using UnityEngine;

public class GrabbableSimple : MonoBehaviour
{
    private bool isHeld = false;
    private Transform handTransform = null;
    private string handName = "";

    private static GrabbableSimple leftHandHolding = null;
    private static GrabbableSimple rightHandHolding = null;

    void OnTriggerEnter(Collider other)
    {
        if (other.name.Contains("Left") || other.name.Contains("Right"))
        {
            handTransform = other.transform;
            handName = other.name.Contains("Left") ? "Left" : "Right";
        }
    }

    void OnTriggerStay(Collider other)
    {
        bool grabbing = (handName == "Right" && PalmDataManager.RightGrabbing) ||
                        (handName == "Left" && PalmDataManager.LeftGrabbing);

        if (!isHeld && handTransform != null && grabbing)
        {
            // 既にその手で掴んでるならスキップ
            if ((handName == "Left" && leftHandHolding != null) ||
                (handName == "Right" && rightHandHolding != null))
            {
                return;
            }

            isHeld = true;

            if (handName == "Left") leftHandHolding = this;
            if (handName == "Right") rightHandHolding = this;
        }
    }

    void Update()
    {
        if (isHeld && handTransform != null)
        {
            transform.position = handTransform.position;

            bool releasing = (handName == "Right" && !PalmDataManager.RightGrabbing) ||
                             (handName == "Left" && !PalmDataManager.LeftGrabbing);

            if (releasing)
            {
                isHeld = false;

                if (handName == "Left" && leftHandHolding == this) leftHandHolding = null;
                if (handName == "Right" && rightHandHolding == this) rightHandHolding = null;

                handTransform = null;
                handName = "";
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.name.Contains(handName))
        {
            handTransform = null;
            handName = "";
        }
    }
}
