using System.Collections.Generic;
using UnityEngine;

public static class ScoreManager
{
    private const string SCORE_KEY = "DualAimScores";
    private const int MAX_RANK = 5;

    public static void SaveScore(int newScore)
    {
        List<int> scores = LoadScores();
        scores.Add(newScore);
        scores.Sort((a, b) => b.CompareTo(a));
        if (scores.Count > MAX_RANK)
            scores = scores.GetRange(0, MAX_RANK);

        PlayerPrefs.SetString(SCORE_KEY, string.Join(",", scores));
        PlayerPrefs.Save();
    }

    public static List<int> LoadScores()
    {
        string data = PlayerPrefs.GetString(SCORE_KEY, "");
        List<int> scores = new List<int>();
        if (!string.IsNullOrEmpty(data))
        {
            string[] parts = data.Split(',');
            foreach (string p in parts)
            {
                if (int.TryParse(p, out int val))
                    scores.Add(val);
            }
        }
        return scores;
    }
}
