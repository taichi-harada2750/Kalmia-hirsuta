using UnityEngine;

public class ConsoleToggleKey : MonoBehaviour
{
    public GameObject consolePanel;

    void Update()
    {
        // Editor上で不要な衝突を避けるなら EditorOnly にしても良い
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            consolePanel.SetActive(!consolePanel.activeSelf);
        }
    }
}
