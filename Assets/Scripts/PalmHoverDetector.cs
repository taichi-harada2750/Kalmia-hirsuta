using UnityEngine;
using UnityEngine.UI;

public class PalmHoverDetector : MonoBehaviour
{
    public RectTransform iconRect; // 対象UI（Imageなど）
    public bool useRightHand = true;
    public float hoverDistance = 50f; // ピクセル距離でHover判定

    private UIHoverEffect hoverEffect;

    void Start()
    {
        hoverEffect = iconRect.GetComponent<UIHoverEffect>();
    }

    void Update()
    {
        Vector2 handPos = useRightHand ? PalmDataManager.RightPalm : PalmDataManager.LeftPalm;
        bool grabbing = useRightHand ? PalmDataManager.RightGrabbing : PalmDataManager.LeftGrabbing;

        Vector2 iconLocalPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            iconRect.parent as RectTransform,
            iconRect.position,
            null,
            out iconLocalPos);

        float dist = Vector2.Distance(handPos, iconLocalPos);

        if (dist < hoverDistance)
        {
            hoverEffect?.SetHover(true);

            if (grabbing)
            {
                hoverEffect?.PlayClickEffect();
            }
        }
        else
        {
            hoverEffect?.SetHover(false);
        }
    }
}

