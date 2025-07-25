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

    private void PlayScoreSE(bool isCorrect)
    {
        if (SoundManager.Instance == null) return;

        if (isCorrect)
            SoundManager.Instance.PlaySE(correctSEKey);
        else
            SoundManager.Instance.PlaySE(wrongSEKey);
    }
}
