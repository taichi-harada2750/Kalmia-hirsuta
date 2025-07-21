using UnityEngine;

public class CloseButton : MonoBehaviour
{
    [Header("閉じる対象のアプリウィンドウ")]
    public GameObject targetApp;

    [Header("アプリ終了制御を行う共通クラス")]
    public CommonAppCloser closer;

    /// <summary>
    /// UIのボタンやIconTriggerから呼び出す用のメソッド
    /// </summary>
    public void OnClickClose()
    {
        if (closer != null && targetApp != null)
        {
            closer.CloseApp(targetApp);
        }
        else
        {
            Debug.LogWarning("[CloseButton] closerまたはtargetAppが設定されていません！");
        }
    }
}
