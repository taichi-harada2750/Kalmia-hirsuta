using UnityEngine;

public class RingUIAutoActivator : MonoBehaviour
{
    public GameObject ringUI;

    void Start()
    {
        if (SceneTransitionManager.Instance != null && SceneTransitionManager.Instance.cameFromResetScene)
        {
            Debug.Log("[RingUIAutoActivator] リセット起動 → RingUIを表示");
            ringUI.SetActive(true);
            SceneTransitionManager.Instance.ClearFlag();
        }
        else
        {
            Debug.Log("[RingUIAutoActivator] 通常起動（RingUIは非表示のまま）");
        }
    }
}
