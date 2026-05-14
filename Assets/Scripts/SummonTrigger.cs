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

    void OnTriggerStay(Collider other)
    {
        if (!isHandInside)
        {
            if (other.name.Contains("Right") || other.name.Contains("Left") || other.name.Contains("Mouse"))
            {
                isHandInside = true;
                if (other.name.Contains("Right")) hoveringHand = "Right";
                else if (other.name.Contains("Left")) hoveringHand = "Left";
                else hoveringHand = "Mouse";
            }
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

        // UnityのSetActive時におけるColliderのOnTriggerEnter発火漏れを防ぐため、
        // 物理的に空間が重なっているかを直接判定する
        bool isPhysicallyTouching = isHandInside;
        Collider myCollider = GetComponent<Collider>();
        if (!isPhysicallyTouching && myCollider != null)
        {
            Collider[] overlaps = Physics.OverlapBox(myCollider.bounds.center, myCollider.bounds.extents, myCollider.transform.rotation);
            foreach (var col in overlaps)
            {
                if (col.name.Contains("Right") || col.name.Contains("Left") || col.name.Contains("Mouse"))
                {
                    isPhysicallyTouching = true;
                    break;
                }
            }
        }

        if (isPhysicallyTouching)
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

        // KIS導入前の「握ったまま触れると即座に発動する」挙動の復元
        if (isHandInside)
        {
            bool isGrabbing = false;
            Vector3 currentPos = Vector3.zero;

            if (hoveringHand == "Right")
            {
                isGrabbing = PalmDataManager.RightGrabbing;
                currentPos = PalmDataManager.RightPalm;
            }
            else if (hoveringHand == "Left")
            {
                isGrabbing = PalmDataManager.LeftGrabbing;
                currentPos = PalmDataManager.LeftPalm;
            }
            else if (hoveringHand == "Mouse")
            {
                isGrabbing = Input.GetMouseButton(0);
                if (Camera.main != null)
                {
                    float depth = Mathf.Abs(Camera.main.transform.position.z);
                    Vector3 mouseScreenPos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, depth);
                    currentPos = Camera.main.ScreenToWorldPoint(mouseScreenPos);
                    currentPos.z = 0f;
                }
                else
                {
                    currentPos = Input.mousePosition;
                }
            }

            if (isGrabbing)
            {
                HandleGrab(currentPos);
            }
        }

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
