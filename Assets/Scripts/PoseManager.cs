using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    public string pauseSceneName = "PauseScene";   // ポーズ用のシーン名
    public string mainSceneName = "MainScene";     // メインシーン名
    private bool isPaused = false;

    private float lastKeyTime = 0f;
    public float inputCooldown = 1f;   // 誤作動防止用のクールダウン(秒)

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            // クールダウン中なら無視
            if (Time.time - lastKeyTime < inputCooldown) return;
            lastKeyTime = Time.time;

            if (!isPaused)
            {
                // PauseScene に完全移動
                SceneManager.LoadScene(pauseSceneName, LoadSceneMode.Single);
                isPaused = true;
            }
            else
            {
                // MainScene に完全移動
                SceneManager.LoadScene(mainSceneName, LoadSceneMode.Single);
                isPaused = false;
            }
        }
    }
}
