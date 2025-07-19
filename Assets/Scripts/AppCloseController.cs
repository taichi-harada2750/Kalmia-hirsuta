using UnityEngine;
using UnityEngine.UI;

public class AppCloseController : MonoBehaviour
{
    [Header("アプリ管理")]
    public GameObject appWindow;            // 起動中のApp（ScratcherWindowなど）
    public GameObject gameCursorGroup;      // ゲーム用カーソル（L/Rセット）
    public GameObject ringUI;               // UIリング本体
    public GameObject uiCursorGroup;        // UI操作用カーソル（L/Rセット）

    [Header("制御")]
    public bool blockClose = false;         // クローズ禁止状態

    void Update()
    {
        // 任意のキーでブロック切り替え（例：Bキー）
        if (Input.GetKeyDown(KeyCode.B))
        {
            blockClose = !blockClose;
            Debug.Log($"[AppCloseController] クローズブロック: {blockClose}");
        }
    }

    /// <summary>
    /// UIの×ボタンなどから呼び出す終了処理
    /// </summary>
    public void CloseApp()
    {
        if (blockClose)
        {
            Debug.Log("[AppCloseController] クローズがブロックされています");
            return;
        }

        appWindow.SetActive(false);
        gameCursorGroup.SetActive(false);

        if (ringUI != null) ringUI.SetActive(true);
        if (uiCursorGroup != null) uiCursorGroup.SetActive(true);

        Debug.Log("[AppCloseController] アプリを閉じてUI操作に戻しました");
    }

    /// <summary>
    /// ゲームからクローズブロックをオンにする（誤作動防止用）
    /// </summary>
    public void SetCloseBlocked(bool blocked)
    {
        blockClose = blocked;
    }
}
