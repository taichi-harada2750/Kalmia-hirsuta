using UnityEngine;

public class CloseButton : MonoBehaviour
{
    [Header("クローズダイアログを呼び出すUI")]
    [SerializeField] private CommonCloseDialog closeDialog;

    [Header("強制リセット処理（2回押し用）")]
    [SerializeField] private SceneResetRequester resetRequester;

    [Header("ホバー演出（任意）")]
    [SerializeField] private UIHoverEffectPulsing effect;

    private bool blockInteraction = false;
    private bool isHovering = false;
    private string hoveringHand = "";

    // Escダブル押し用
    private float lastEscPressedTime = -99f;
    private const float escDoublePressThreshold = 1.5f; // 秒

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

        if (isHovering)
        {
            bool isGrabbing = (hoveringHand == "Right" && PalmDataManager.RightGrabbing) ||
                              (hoveringHand == "Left" && PalmDataManager.LeftGrabbing);

            if (isGrabbing)
            {
                effect?.PlayClickEffect();
                closeDialog?.ShowDialog();
                isHovering = false;
                hoveringHand = "";
            }
        }
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

    // 外部制御
    public void Block() => blockInteraction = true;
    public void Allow() => blockInteraction = false;
    public void SetBlocked(bool blocked) => blockInteraction = blocked;
    public bool IsBlocked => blockInteraction;
}
