using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class ScoreDisplay : MonoBehaviour
{
    [Header("ランキング表示先")]
    public TMP_Text rankingText;
    [Tooltip("上位何件まで表示するか")]
    public int displayCount = 5;

    void Start()
    {
        UpdateRanking();
    }

    public void UpdateRanking()
    {
        if (rankingText == null) return;

        List<int> scores = ScoreManager.LoadScores();
        rankingText.text = "Top Scores\n";

        if (scores.Count == 0)
        {
            rankingText.text += "（スコアがありません）";
            return;
        }

        for (int i = 0; i < Mathf.Min(displayCount, scores.Count); i++)
        {
            rankingText.text += $"{i + 1}. {scores[i]} pts\n";
        }
    }
}
