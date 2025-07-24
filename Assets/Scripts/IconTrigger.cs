using UnityEngine;
using UnityEngine.Events;

public class IconTrigger : MonoBehaviour
{
    public UIHoverEffectPulsing effect;

    [Tooltip("このアイコンが選択されたときに呼び出す処理")]
    public UnityEvent onClick;

    [Header("効果音キー（SoundManager側に登録された名前）")]
    public string clickSEKey = "click";

    [Header("誤作動防止設定")]
    public bool requireReleaseBeforeClick = false;

    private bool hasEnteredSinceSummon = false;
    private bool hasBeenReleased = false;

    private bool isHovering = false;
    private string hoveringHand = "";

    void OnTriggerEnter(Collider other)
    {
        if (other.name.Contains("Right") || other.name.Contains("Left"))
        {
            hoveringHand = other.name.Contains("Right") ? "Right" : "Left";
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

    void Update()
    {
        if (!isHovering) return;

        bool isGrabbing = (hoveringHand == "Right" && PalmDataManager.RightGrabbing) ||
                          (hoveringHand == "Left" && PalmDataManager.LeftGrabbing);

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
            effect?.PlayClickEffect();

            if (!string.IsNullOrEmpty(clickSEKey))
                SoundManager.Instance.PlaySE(clickSEKey); // ← 名前指定でSE再生！

            onClick?.Invoke();

            isHovering = false;
            hoveringHand = "";
            hasEnteredSinceSummon = false;
            hasBeenReleased = false;
        }
    }
}
