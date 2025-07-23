using UnityEngine;

public class TargetResetter : MonoBehaviour
{
    [Tooltip("削除対象のタグ名（複数指定可能）")]
    public string[] targetTags = { "Target" };

    [Tooltip("スコアもリセットするか")]
    public bool resetScore = true;

    [Tooltip("実行時にログを出す")]
    public bool logOnReset = true;

    [Tooltip("リセットを遅らせる秒数（例：2秒）")]
    public float resetDelay = 2f;

    public void ResetAll()
    {
        CancelInvoke(nameof(PerformReset));
        Invoke(nameof(PerformReset), resetDelay);
    }

    private void PerformReset()
    {
        int totalCount = 0;
        foreach (string tag in targetTags)
        {
            GameObject[] targets = GameObject.FindGameObjectsWithTag(tag);
            foreach (var obj in targets)
            {
                Destroy(obj);
                totalCount++;
            }
        }

        if (resetScore)
        {
            SortGameManager.Instance.ResetScore();
        }

        if (logOnReset)
        {
            Debug.Log($"[TargetResetter] {totalCount}個のターゲットを削除しました。スコアもリセット: {resetScore}");
        }
    }
}
