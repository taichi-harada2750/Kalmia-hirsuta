using UnityEngine;
using UnityEngine.Events;
using KIS.Output;

public class IconTrigger : MonoBehaviour
{
    public UIHoverEffectPulsing effect;

    [Tooltip("このアイコンが選択されたときに呼び出す処理")]
    public UnityEvent onClick;

    [Header("効果音キー（SoundManager側に登録された名前）")]
    public string clickSEKey = "click";

    [Header("誤作動防止設定")]
    public bool requireReleaseBeforeClick = false;

    [Header("KIS Adapter")]
    public UnityUIOutputAdapter kisAdapter;

    private bool hasEnteredSinceSummon = false;
    private bool hasBeenReleased = false;

    private bool isHovering = false;
    private string hoveringHand = "";

    void Awake()
    {
        if (kisAdapter == null) kisAdapter = FindObjectOfType<UnityUIOutputAdapter>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.name.Contains("Right") || other.name.Contains("Left") || other.name.Contains("Mouse"))
        {
            if (other.name.Contains("Right")) hoveringHand = "Right";
            else if (other.name.Contains("Left")) hoveringHand = "Left";
            else hoveringHand = "Mouse";
            
            isHovering = true;
            hasEnteredSinceSummon = true;
            hasBeenReleased = false;
            effect?.SetHover(true);
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (!isHovering && (other.name.Contains("Right") || other.name.Contains("Left") || other.name.Contains("Mouse")))
        {
            if (other.name.Contains("Right")) hoveringHand = "Right";
            else if (other.name.Contains("Left")) hoveringHand = "Left";
            else hoveringHand = "Mouse";

            isHovering = true;
            effect?.SetHover(true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.name.Contains(hoveringHand))
        {
            isHovering = false;
            hoveringHand = "";
            effect?.SetHover(false);
        }
    }

    void OnEnable()
    {
        if (kisAdapter != null) kisAdapter.OnGrabDetected += HandleGrab;
    }

    void OnDisable()
    {
        if (kisAdapter != null) kisAdapter.OnGrabDetected -= HandleGrab;
        isHovering = false;
        hoveringHand = "";
        hasEnteredSinceSummon = false;
        hasBeenReleased = false;
    }

    private void HandleGrab(Vector3 grabPos)
    {
        bool isPhysicallyTouching = isHovering;
        Collider myCollider = GetComponent<Collider>();
        
        if (!isPhysicallyTouching && myCollider != null)
        {
            // isHoveringが何らかの理由(OnTriggerStayの不発など)でfalseになっていても、
            // 物理的にカーソルが重なっていれば許可するためのフォールバックチェック
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

        if (!isPhysicallyTouching) return;

        // KIS側の GrabIntent は「握った瞬間」にしか発火しないため、
        // requireReleaseBeforeClick の挙動を自然に満たします。
        
        effect?.PlayClickEffect();

        if (!string.IsNullOrEmpty(clickSEKey))
            SoundManager.Instance.PlaySE(clickSEKey);

        onClick?.Invoke();

        isHovering = false;
        hoveringHand = "";
        hasEnteredSinceSummon = false;
        hasBeenReleased = false;
    }

    void Update()
    {
        if (!isHovering) return;

        // KIS導入前の「握ったままアイコンに触れると即座にクリックされる」挙動の復元
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

        if (requireReleaseBeforeClick)
        {
            if (hasEnteredSinceSummon && !hasBeenReleased && !isGrabbing)
            {
                hasBeenReleased = true;
                return;
            }
            if (!hasBeenReleased) return;
        }

        if (isGrabbing)
        {
            HandleGrab(currentPos);
        }
    }
}
