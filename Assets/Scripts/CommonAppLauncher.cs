using UnityEngine;
using UnityEngine.UI;

public class CommonAppLauncher : MonoBehaviour
{
    public GameObject ringUI;
    public PalmCursorManager cursorManager;

    [Header("起動に必要な構成")]
    public GameObject gameWindow;          // ゲーム用UI一式（スクラッチウィンドウなど）
    public GameObject gameCursorGroup;     // ゲーム用カーソル（両手セット）
    public Button closeButton;             // ×ボタン
    public MonoBehaviour gameLogicScript;  // ScratcherManagerなどを想定

    private bool isRunning = false;

    public void LaunchApp()
    {
        if (isRunning) return;

        ringUI.SetActive(false);
        cursorManager.SetMode(PalmCursorManager.CursorMode.Game);

        gameWindow.SetActive(true);
        gameCursorGroup.SetActive(true);
        closeButton.gameObject.SetActive(true);

        if (gameLogicScript != null)
        {
            var method = gameLogicScript.GetType().GetMethod("StartApp");
            method?.Invoke(gameLogicScript, null);
        }

        closeButton.onClick.RemoveAllListeners();
        closeButton.onClick.AddListener(CloseApp);

        isRunning = true;
    }

    public void CloseApp()
    {
        ringUI.SetActive(true);
        cursorManager.SetMode(PalmCursorManager.CursorMode.UI);

        gameWindow.SetActive(false);
        gameCursorGroup.SetActive(false);
        closeButton.gameObject.SetActive(false);

        isRunning = false;
    }
}
