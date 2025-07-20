using UnityEngine;

public class GrabbableObject : MonoBehaviour
{
    public bool IsBeingHeld { get; private set; }
    private Transform followTarget;

    public void Grab(Transform hand)
    {
        IsBeingHeld = true;
        followTarget = hand;
    }

    public void Release()
    {
        IsBeingHeld = false;
        followTarget = null;
    }

    void Update()
    {
        if (IsBeingHeld && followTarget != null)
        {
            transform.position = followTarget.position;
        }
    }
}
