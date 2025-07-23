using UnityEngine;

public class RingUISummonerOnReset : MonoBehaviour
{
        [SerializeField]
    public SummonTrigger summonTrigger;

    void Start()
    {
        if (SceneTransitionManager.Instance != null && SceneTransitionManager.Instance.cameFromResetScene)
        {
            Debug.Log("[RingUISummonerOnReset] リセット起動 → SummonTrigger.ShowRingUI を実行");
            summonTrigger.ShowRingUI(); // ← これで演出付きでリングUIが展開される！
            SceneTransitionManager.Instance.ClearFlag();
        }
    }
}
