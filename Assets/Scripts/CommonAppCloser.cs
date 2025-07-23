using UnityEngine;
using UnityEngine.SceneManagement;

public class CommonAppCloser : MonoBehaviour
{
    public void CloseApp(GameObject appWindow)
    {
        Debug.Log($"[CommonAppCloser] アプリ {appWindow.name} を終了し、シーンリセット開始");

        SceneTransitionManager.Instance.MarkAsSceneReset(); // ← フラグON
        SceneManager.LoadScene("LoadingScene");
    }
}
