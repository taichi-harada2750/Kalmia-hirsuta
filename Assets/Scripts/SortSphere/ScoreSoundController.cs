using UnityEngine;

public class ScoreSoundController : MonoBehaviour
{
    [Header("SoundManagerに登録されたキー")]
    public string correctSEKey = "success";
    public string wrongSEKey = "fail";

    void OnEnable()
    {
        SortGameManager.OnScoreChanged += PlayScoreSE;
    }

    void OnDisable()
    {
        SortGameManager.OnScoreChanged -= PlayScoreSE;
    }

    private static int lastPlayFrame = -1;

    private void PlayScoreSE(bool isCorrect)
    {
        if (SoundManager.Instance == null) return;

        // 同一フレームでの重複再生（爆音化）を防止
        if (Time.frameCount == lastPlayFrame) return;
        lastPlayFrame = Time.frameCount;

        if (isCorrect)
            SoundManager.Instance.PlaySE(correctSEKey);
        else
            SoundManager.Instance.PlaySE(wrongSEKey);
    }
}
