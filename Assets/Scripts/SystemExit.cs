using UnityEngine;

public class SystemExit : MonoBehaviour
{
    /// <summary>
    /// アプリケーションを終了する
    /// </summary>
    public void ExitApplication()
    {
#if UNITY_EDITOR
        // エディタ上では再生モードを停止
        UnityEditor.EditorApplication.isPlaying = false;
#else
        // ビルド版ではウィンドウごと終了
        Application.Quit();
#endif
    }
}
