using UnityEngine;
using UnityEngine.Events;

public class CommonAppCloser : MonoBehaviour
{
    [Header("UI制御")]
    public GameObject ringUI;
    public PalmCursorManager cursorManager;
    public GameObject gameCursorGroup;

    [Header("終了時に実行される処理（オプション）")]
    public UnityEvent onCloseExtras; // ← 追加ポイント！

    public void CloseApp(GameObject appWindow)
    {
        Debug.Log($"[CommonAppCloser] アプリ {appWindow.name} を終了処理中");

        if (appWindow != null) appWindow.SetActive(false);
        if (gameCursorGroup != null) gameCursorGroup.SetActive(false);

        if (ringUI != null) ringUI.SetActive(true);
        if (cursorManager != null) cursorManager.SetUICursorActive(true);

        Debug.Log("[CommonAppCloser] UI操作に復帰しました");

        // 任意の追加処理（リセット等）を実行
        onCloseExtras?.Invoke(); // ← ここ！
    }
}
