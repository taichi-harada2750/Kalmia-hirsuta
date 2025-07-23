using UnityEngine;
using UnityEngine.SceneManagement;

public class ResetLoader : MonoBehaviour
{
    [Header("再遷移先のシーン名（MainSceneなど）")]
    [SerializeField] private string destinationSceneName = "MainScene";

    void Start()
    {
        if (string.IsNullOrEmpty(destinationSceneName))
        {
            Debug.LogError("[ResetLoader] 遷移先シーン名が設定されていません！");
            return;
        }

        Debug.Log($"[ResetLoader] シーンを {destinationSceneName} に遷移します");
        SceneManager.LoadScene(destinationSceneName);
    }
}
