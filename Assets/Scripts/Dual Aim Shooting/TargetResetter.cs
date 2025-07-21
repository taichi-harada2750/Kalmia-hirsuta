using UnityEngine;

public class TargetResetter : MonoBehaviour
{
    [Tooltip("削除対象のタグ名（デフォルト: Target）")]
    public string targetTag = "Target";

    [Tooltip("スコアもリセットするか")]
    public bool resetScore = true;

    [Tooltip("実行時にログを出す")]
    public bool logOnReset = true;

    [Tooltip("リセットを遅らせる秒数（例：2秒）")]
    public float resetDelay = 2f;

    // 外部から呼び出す用
    public void ResetAll()
    {
        CancelInvoke(nameof(PerformReset));
        Invoke(nameof(PerformReset), resetDelay);
    }

    private void PerformReset()
    {
        GameObject[] targets = GameObject.FindGameObjectsWithTag(targetTag);
        foreach (var obj in targets)
        {
            Destroy(obj);
        }

        if (resetScore)
        {
            SortGameManager.Instance.ResetScore();
        }

        if (logOnReset)
        {
            Debug.Log($"[TargetResetter] {targets.Length}個のターゲットを削除しました。スコアもリセット: {resetScore}");
        }
    }
}
