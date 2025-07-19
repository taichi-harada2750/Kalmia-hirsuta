using UnityEngine;

public class PalmCursorManager : MonoBehaviour
{
    public GameObject uiCursorLeft;
    public GameObject uiCursorRight;

    public void SetUICursorActive(bool active)
    {
        if (uiCursorLeft != null) uiCursorLeft.SetActive(active);
        if (uiCursorRight != null) uiCursorRight.SetActive(active);
    }
}
