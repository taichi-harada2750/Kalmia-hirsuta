using UnityEngine;

public class SceneTransitionManager : MonoBehaviour
{
    public static SceneTransitionManager Instance;

    public bool cameFromResetScene = false;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void MarkAsSceneReset()
    {
        cameFromResetScene = true;
    }

    public void ClearFlag()
    {
        cameFromResetScene = false;
    }
}
