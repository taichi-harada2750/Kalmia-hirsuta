using UnityEngine;

public class PalmCursorManager : MonoBehaviour
{
    public GameObject uiCursorLeft;
    public GameObject uiCursorRight;
    public GameObject uiCursorMouse;

    void Awake()
    {
        if (uiCursorMouse == null)
        {
            uiCursorMouse = GameObject.Find("MouseCursor");
        }
    }

    public void SetUICursorActive(bool active)
    {
        if (uiCursorLeft != null) uiCursorLeft.SetActive(active);
        if (uiCursorRight != null) uiCursorRight.SetActive(active);
        if (uiCursorMouse != null) uiCursorMouse.SetActive(active);
    }
}
