using UnityEngine;

public class GrabbableDestroyer : MonoBehaviour
{
    public enum ObjectType { Good, Bad }
    public ObjectType type = ObjectType.Good;

    private bool wasLeftGrabbing = false;
    private bool wasRightGrabbing = false;

    void OnTriggerStay(Collider other)
    {
        if (other.name.Contains("Left"))
        {
            bool nowGrabbing = PalmDataManager.LeftGrabbing;
            if (nowGrabbing && !wasLeftGrabbing)
            {
                TryDestroy("Left");
            }
            wasLeftGrabbing = nowGrabbing;
        }
        else if (other.name.Contains("Right"))
        {
            bool nowGrabbing = PalmDataManager.RightGrabbing;
            if (nowGrabbing && !wasRightGrabbing)
            {
                TryDestroy("Right");
            }
            wasRightGrabbing = nowGrabbing;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.name.Contains("Left")) wasLeftGrabbing = false;
        if (other.name.Contains("Right")) wasRightGrabbing = false;
    }

    void TryDestroy(string hand)
    {
        bool isCorrect = (type == ObjectType.Good);
        SortGameManager.Instance.AddScore(isCorrect);
        Debug.Log($"[{hand}] {(isCorrect ? "成功" : "減点")} → {gameObject.name}");
        Destroy(gameObject);
    }
}
