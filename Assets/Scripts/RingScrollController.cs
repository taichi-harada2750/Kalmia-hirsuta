using UnityEngine;

public class RingScrollController : MonoBehaviour
{
    public Transform ringUI; // 回転させる対象（Canvas内のリングUI）
    public float rotationSpeed = 300f;

    private bool isGrabbing = false;
    private Vector3 lastHandPos;

    void Update()
    {
        Vector3 handPos = PalmDataManager.LeftPalm; // or RightPalm
        bool grabbing = PalmDataManager.LeftGrabbing;

        if (!isGrabbing && grabbing)
        {
            isGrabbing = true;
            lastHandPos = handPos;
        }

        if (isGrabbing && grabbing)
        {
            float deltaX = handPos.x - lastHandPos.x;

            // Z軸（上方向）回転
            ringUI.Rotate(0f, 0f, -deltaX * rotationSpeed * Time.deltaTime); // ← Z軸中心に回す


            lastHandPos = handPos;
        }

        if (isGrabbing && !grabbing)
        {
            isGrabbing = false;
        }
    }
}
