using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneResetRequester : MonoBehaviour
{
    [SerializeField] private string loadingSceneName = "LoadingScene";

    public void RequestReset()
    {
        if (SceneTransitionManager.Instance != null)
        {
            SceneTransitionManager.Instance.MarkAsSceneReset();
        }

        Debug.Log("[SceneResetRequester] リセットを要求 → LoadingSceneへ");
        SceneManager.LoadScene(loadingSceneName);
    }
}
