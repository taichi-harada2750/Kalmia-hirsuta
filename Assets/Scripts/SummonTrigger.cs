using UnityEngine;

public class SummonTrigger : MonoBehaviour
{
    public GameObject ringUI;
    public GameObject summonButton; // ← UI召喚ボタン（自身）
    public GameObject resetButton;  // ← リセットボタン

    private bool isHandInside = false;

    private float inputBlockTimer = 0f; // 入力ブロック用タイマー
    public float inputBlockDuration = 0.6f; // ブロック時間（秒）

    void OnTriggerEnter(Collider other)
    {
        if (other.name.Contains("Right"))
        {
            isHandInside = true;
            Debug.Log("🟠 右手オブジェクトがTriggerに入りました（中身はLeftHand）");
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.name.Contains("Right"))
        {
            isHandInside = false;
            Debug.Log("⚪ 右手オブジェクトがTriggerから出ました");
        }
    }

    void Update()
    {
        // 入力ブロック中は何もしない
        if (inputBlockTimer > 0f)
        {
            inputBlockTimer -= Time.deltaTime;
            return;
        }

        if (isHandInside && PalmDataManager.LeftGrabbing)
        {
            ShowRingUI();
            isHandInside = false;
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

    // 外部から呼び出して、一定時間グラブを無効化
    public void BlockInputForSeconds(float seconds)
    {
        inputBlockTimer = seconds;
    }
}
