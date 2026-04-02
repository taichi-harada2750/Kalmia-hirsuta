using UnityEngine;
using UnityEngine.Events;
using KIS.Output;

public class IconTriggerWithClose : MonoBehaviour
{
    public UIHoverEffectPulsing effect;

    [Tooltip("このアイコンが選択されたときに呼び出す処理")]
    public UnityEvent onClick;

    [Header("効果音キー（SoundManager側に登録された名前）")]
    public string clickSEKey = "click";

    [Header("誤作動防止設定")]
    public bool requireReleaseBeforeClick = false;

    [Header("終了処理設定")]
    [Tooltip("このウィンドウを閉じる対象（アクティブをfalseにする）")]
    public GameObject targetWindow;

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
    }

    private void HandleGrab(Vector3 grabPos)
    {
        if (!isHovering) return;

        effect?.PlayClickEffect();

        if (!string.IsNullOrEmpty(clickSEKey))
            SoundManager.Instance.PlaySE(clickSEKey);

        // まず既存のonClickイベントを呼び出す
        onClick?.Invoke();

        // その後、対象ウィンドウを閉じる
        if (targetWindow != null)
        {
            targetWindow.SetActive(false);
        }

        isHovering = false;
        hoveringHand = "";
        hasEnteredSinceSummon = false;
        hasBeenReleased = false;
    }

    void Update()
    {
        // KISのイベント駆動になったためUpdate内での直接のGrab判定処理は削除
    }
}
