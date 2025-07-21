using UnityEngine;
using UnityEngine.UI;
using System.Collections;


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

        // Escキーでクローズ
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Debug.Log("[AppCloseController] Escキーでクローズを試行");
            CloseApp();
        }
    }


    /// <summary>
    /// UIの×ボタンなどから呼び出す終了処理
    /// </summary>
[Header("終了時に実行するイベント")]
public UnityEngine.Events.UnityEvent onAppClosed;

public void CloseApp()
{
    if (blockClose)
    {
        Debug.Log("[AppCloseController] クローズがブロックされています");
        return;
    }

    Debug.Log("[AppCloseController] CloseApp() 開始");

    if (appWindow != null) appWindow.SetActive(false);
    if (gameCursorGroup != null) gameCursorGroup.SetActive(false);

    if (ringUI != null)
    {
        ringUI.SetActive(true);
        Debug.Log("[AppCloseController] ringUI を表示しました");
    }
    else
    {
        Debug.LogWarning("[AppCloseController] ringUI が設定されていません！");
    }

    if (uiCursorGroup != null)
    {
        uiCursorGroup.SetActive(true);
        Debug.Log("[AppCloseController] uiCursorGroup を表示しました");
    }
    else
    {
        Debug.LogWarning("[AppCloseController] uiCursorGroup が設定されていません！");
    }

    onAppClosed?.Invoke();

    // 最後に自身を非表示（他が反映されてから）
    StartCoroutine(DeactivateSelf());
}

private IEnumerator DeactivateSelf()
{
    yield return null;
    gameObject.SetActive(false);
    Debug.Log("[AppCloseController] 自分自身を非アクティブにしました");
}


private IEnumerator CloseSequence()
{
    appWindow.SetActive(false);
    gameCursorGroup?.SetActive(false);

    yield return null; // フレームを跨いで確実に状態反映

    ringUI?.SetActive(true);
    uiCursorGroup?.SetActive(true);

    Debug.Log("[AppCloseController] アプリを閉じてUI操作に戻しました");

    onAppClosed?.Invoke();

    yield return null;
    gameObject.SetActive(false); // 最後に無効化
}




    /// <summary>
    /// ゲームからクローズブロックをオンにする（誤作動防止用）
    /// </summary>
    public void SetCloseBlocked(bool blocked)
    {
        blockClose = blocked;
    }
}
