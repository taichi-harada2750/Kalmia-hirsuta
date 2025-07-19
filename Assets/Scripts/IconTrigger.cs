using UnityEngine;
using UnityEngine.Events;

public class IconTrigger : MonoBehaviour
{
    public UIHoverEffectPulsing effect;

    [Tooltip("このアイコンが選択されたときに呼び出す処理")]
    public UnityEvent onClick;

    private bool isHovering = false;
    private string hoveringHand = ""; // "Left" または "Right"

    void OnTriggerEnter(Collider other)
    {
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
        if (other.name.Contains(hoveringHand))
        {
            isHovering = false;
            hoveringHand = "";
            effect?.SetHover(false);
        }
    }

    void Update()
    {
        if (isHovering)
        {
            bool isGrabbing = (hoveringHand == "Right" && PalmDataManager.RightGrabbing) ||
                              (hoveringHand == "Left" && PalmDataManager.LeftGrabbing);

            if (isGrabbing)
            {
                effect?.PlayClickEffect();
                onClick?.Invoke();
                isHovering = false;
                hoveringHand = "";
            }
        }
    }
}
