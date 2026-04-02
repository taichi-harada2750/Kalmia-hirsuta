using UnityEngine;
using KIS.Output;

public class SummonTrigger : MonoBehaviour
{
    public GameObject ringUI;
    public GameObject summonButton;
    public GameObject resetButton;

    [Header("KIS Adapter")]
    public UnityUIOutputAdapter kisAdapter;

    private bool isHandInside = false;
    private string hoveringHand = ""; // "Left" または "Right"

    private float inputBlockTimer = 0f;
    public float inputBlockDuration = 0.6f;

    void Awake()
    {
        if (kisAdapter == null) kisAdapter = FindObjectOfType<UnityUIOutputAdapter>();
    }

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
        else if (other.name.Contains("Mouse"))
        {
            isHandInside = true;
            hoveringHand = "Mouse";
            Debug.Log("🟠 マウスオブジェクトがTriggerに入りました");
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

    void OnEnable()
    {
        if (kisAdapter != null) kisAdapter.OnGrabDetected += HandleGrab;
    }

    void OnDisable()
    {
        if (kisAdapter != null) kisAdapter.OnGrabDetected -= HandleGrab;
    }

    private void HandleGrab(Vector3 grabPos)
    {
        if (inputBlockTimer > 0f) return;

        if (isHandInside)
        {
            ShowRingUI();
            isHandInside = false;
            hoveringHand = "";
        }
    }

    void Update()
    {
        if (inputBlockTimer > 0f)
        {
            inputBlockTimer -= Time.deltaTime;
            return;
        }

        // KISのイベント駆動になったためUpdate内での直接のGrab監視は削除

        if (Input.GetKeyDown(KeyCode.Space))
        {
            ShowRingUI();
        }
    }

    public void ShowRingUI()
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
