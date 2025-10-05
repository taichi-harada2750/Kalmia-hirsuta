using UnityEngine;

public class HardScoreHandler : MonoBehaviour
{
    public static HardScoreHandler Instance;

    [Header("Hardモード設定")]
    public bool hardModeActive = false; // ← DualAimControllerから切り替える
    public int hardPenaltyPoint = -3;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    /// <summary>
    /// HardBad専用の減点処理
    /// </summary>
    public void AddHardPenalty()
    {
        if (!hardModeActive) return; // OFF時は無効
        SortGameManager.Instance.AddScore(false); // ← SortGameManagerの減点呼び出し
        SortGameManager.Instance.score += (hardPenaltyPoint + 1);
        // ↑ AddScore(false) で -1 減るので、合計 -3 にしたい場合は +(-3+1) = -2 で調整
        SortGameManager.Instance.GetScore(); // スコア反映（UIは自動更新）

        Debug.Log($"[HardScoreHandler] Hard減点 {hardPenaltyPoint}点 (合計 {SortGameManager.Instance.score})");
    }
}
