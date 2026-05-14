using UnityEngine;
using KIS.Output;

public class CloseButton : MonoBehaviour
{
    [Header("クローズダイアログを呼び出すUI")]
    [SerializeField] private CommonCloseDialog closeDialog;

    [Header("強制リセット処理（2回押し用）")]
    [SerializeField] private SceneResetRequester resetRequester;

    [Header("ホバー演出（任意）")]
    [SerializeField] private UIHoverEffectPulsing effect;

    [Header("KIS Adapter")]
    public UnityUIOutputAdapter kisAdapter;

    private bool blockInteraction = false;
    private bool isHovering = false;
    private string hoveringHand = "";

    // Escダブル押し用
    private float lastEscPressedTime = -99f;
    private const float escDoublePressThreshold = 1.5f; // 秒

    void Awake()
    {
        if (kisAdapter == null) kisAdapter = FindObjectOfType<UnityUIOutputAdapter>();
    }

    void Update()
    {
        // Escキー対応
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            float now = Time.time;
            if (now - lastEscPressedTime < escDoublePressThreshold)
            {
                Debug.Log("[CloseButton] Escキー2回押しで強制リセットします");
                resetRequester?.RequestReset();
            }
            else
            {
                Debug.Log("[CloseButton] Escキー1回目 → クローズダイアログ表示");
                closeDialog?.ShowDialog();
                lastEscPressedTime = now;
            }

            return; // Esc入力時は他の処理は無視
        }

        // 通常Hover+Grab操作（ブロック中は無効）
        if (blockInteraction) return;

        // KISのイベント駆動になったためUpdate内での直接のGrab判定処理は削除
    }

    void OnTriggerEnter(Collider other)
    {
        if (blockInteraction) return;

        if (other.name.Contains("Right"))
        {
            hoveringHand = "Right";
            isHovering = true;
            effect?.SetHover(true);
        }
        else if (other.name.Contains("Left"))
        {
            hoveringHand = "Left";
            isHovering = true;
            effect?.SetHover(true);
        }
        else if (other.name.Contains("Mouse"))
        {
            hoveringHand = "Mouse";
            isHovering = true;
            effect?.SetHover(true);
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (blockInteraction) return;

        if (!isHovering)
        {
            if (other.name.Contains("Right") || other.name.Contains("Left") || other.name.Contains("Mouse"))
            {
                isHovering = true;
                effect?.SetHover(true);
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (blockInteraction) return;

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
        if (blockInteraction) return;

        bool isPhysicallyTouching = isHovering;
        Collider myCollider = GetComponent<Collider>();
        if (myCollider != null)
        {
            Vector3 closestPoint = myCollider.ClosestPoint(grabPos);
            if (Vector3.Distance(closestPoint, grabPos) <= 10f)
            {
                isPhysicallyTouching = true;
            }
        }

        if (isPhysicallyTouching)
        {
            effect?.PlayClickEffect();
            closeDialog?.ShowDialog();
            isHovering = false;
            hoveringHand = "";
        }
    }

    // 外部制御
    public void Block() => blockInteraction = true;
    public void Allow() => blockInteraction = false;
    public void SetBlocked(bool blocked) => blockInteraction = blocked;
    public bool IsBlocked => blockInteraction;
}
