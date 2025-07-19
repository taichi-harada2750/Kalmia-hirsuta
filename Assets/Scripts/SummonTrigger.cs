using UnityEngine;

public class SummonTrigger : MonoBehaviour
{
    public GameObject ringUI;
    public GameObject summonButton;
    public GameObject resetButton;

    private bool isHandInside = false;
    private string hoveringHand = ""; // "Left" または "Right"

    private float inputBlockTimer = 0f;
    public float inputBlockDuration = 0.6f;

    void OnTriggerEnter(Collider other)
    {
        if (other.name.Contains("Right"))
        {
            isHandInside = true;
            hoveringHand = "Right";
            Debug.Log("🟠 右手オブジェクトがTriggerに入りました");
        }
        else if (other.name.Contains("Left"))
        {
            isHandInside = true;
            hoveringHand = "Left";
            Debug.Log("🟠 左手オブジェクトがTriggerに入りました");
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.name.Contains(hoveringHand))
        {
            isHandInside = false;
            hoveringHand = "";
            Debug.Log("⚪ " + other.name + " がTriggerから出ました");
        }
    }

    void Update()
    {
        if (inputBlockTimer > 0f)
        {
            inputBlockTimer -= Time.deltaTime;
            return;
        }

        if (isHandInside)
        {
            bool isGrabbing = (hoveringHand == "Right" && PalmDataManager.RightGrabbing) ||
                              (hoveringHand == "Left" && PalmDataManager.LeftGrabbing);

            if (isGrabbing)
            {
                ShowRingUI();
                isHandInside = false;
                hoveringHand = "";
            }
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            ShowRingUI();
        }
    }

    void ShowRingUI()
    {
        ringUI.SetActive(true);
        ringUI.transform.position = transform.position;

        var animator = ringUI.GetComponent<RingUIAnimator_RectTransform>();
        if (animator != null)
        {
            animator.PlaySummonAnimation();
        }

        if (summonButton != null) summonButton.SetActive(false);
        if (resetButton != null) resetButton.SetActive(true);

        Debug.Log("🟢 リングUI表示 & ボタン切り替え");
    }

    public void BlockInputForSeconds(float seconds)
    {
        inputBlockTimer = seconds;
    }
}
