using UnityEngine;
using UnityEngine.UI;

public class CommonAppLauncher : MonoBehaviour
{
    [Header("共通UI・カーソル")]
    public GameObject ringUI;                        // リングUI（起動前に表示される）
    public PalmCursorManager cursorManager;          // UIカーソル制御用

    [Header("ゲーム専用")]
    public GameObject gameWindow;                    // アプリ本体（ウィンドウ）
    public GameObject gameCursorGroup;               // ゲームカーソル（左右手セット）
    public GameObject closeButtonObject;             // ×ボタン（IconTrigger付き）

    [Header("ゲーム処理スクリプト")]
    public MonoBehaviour gameLogicScript;            // ScratcherManagerなど

    private bool isRunning = false;

    public void LaunchApp()
    {
        if (isRunning) return;

        // UIカーソル非表示、リング非表示
        if (cursorManager != null) cursorManager.SetUICursorActive(false);
        if (ringUI != null) ringUI.SetActive(false);

        // ゲーム系ON
        if (gameCursorGroup != null) gameCursorGroup.SetActive(true);
        if (gameWindow != null) gameWindow.SetActive(true);
        if (closeButtonObject != null) closeButtonObject.SetActive(true);

        // ゲーム初期化（StartAppメソッドを呼ぶ）
        if (gameLogicScript != null)
        {
            var method = gameLogicScript.GetType().GetMethod("StartApp");
            method?.Invoke(gameLogicScript, null);
        }

        isRunning = true;
        Debug.Log("[CommonAppLauncher] アプリを起動しました。");
    }

    public void CloseApp()
    {
        // ゲームUIやカーソルをOFF
        if (gameWindow != null) gameWindow.SetActive(false);
        if (gameCursorGroup != null) gameCursorGroup.SetActive(false);
        if (closeButtonObject != null) closeButtonObject.SetActive(false);

        // UIカーソルとリングUIを戻す
        if (cursorManager != null) cursorManager.SetUICursorActive(true);
        if (ringUI != null) ringUI.SetActive(true);

        isRunning = false;
        Debug.Log("[CommonAppLauncher] アプリを終了してUIに戻りました。");
    }
}
