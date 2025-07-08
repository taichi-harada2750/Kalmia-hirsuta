using UnityEngine;
using UnityEngine.Events;

public class IconTrigger : MonoBehaviour
{
    public UIHoverEffectPulsing effect;

    [Tooltip("このアイコンが選択されたときに呼び出す処理")]
    public UnityEvent onClick;

    private bool isHovering = false;

    void OnTriggerEnter(Collider other)
    {
        if (other.name.Contains("Right"))
        {
            isHovering = true;
            effect?.SetHover(true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.name.Contains("Right"))
        {
            isHovering = false;
            effect?.SetHover(false);
        }
    }

    void Update()
    {
        if (isHovering && (PalmDataManager.LeftGrabbing || PalmDataManager.RightGrabbing))
        {
            effect?.PlayClickEffect();
            onClick?.Invoke(); // ← 後で「ウィンドウを開く」とかに使える
            isHovering = false; // 繰り返し防止
        }
    }
}
