using System.Collections.Generic;
using UnityEngine;

public static class ScoreManager
{
    private const int MAX_RANK = 5;

    // 登録されたゲームキー一覧（表示名, PlayerPrefsキー）
    private static readonly (string displayName, string key)[] GameEntries = new[]
    {
        ("Dual Aim Shooting", "DualAimScores"),
        ("Fluid Slime",       "FluidSlimeScores"),
    };

    /// <summary>
    /// 登録されている全ゲームの（表示名, キー）一覧を返す
    /// </summary>
    public static (string displayName, string key)[] GetAllGameEntries()
    {
        return GameEntries;
    }

    public static void SaveScore(int newScore, string gameKey = "DualAimScores")
    {
        List<int> scores = LoadScores(gameKey);
        scores.Add(newScore);
        scores.Sort((a, b) => b.CompareTo(a));
        if (scores.Count > MAX_RANK)
            scores = scores.GetRange(0, MAX_RANK);

        PlayerPrefs.SetString(gameKey, string.Join(",", scores));
        PlayerPrefs.Save();
    }

    public static List<int> LoadScores(string gameKey = "DualAimScores")
    {
        string data = PlayerPrefs.GetString(gameKey, "");
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

    /// <summary>
    /// 指定したゲームのスコアをリセットする
    /// </summary>
    public static void ResetScores(string gameKey)
    {
        PlayerPrefs.DeleteKey(gameKey);
        PlayerPrefs.Save();
    }

    /// <summary>
    /// 全ゲームのスコアをリセットする
    /// </summary>
    public static void ResetAllScores()
    {
        foreach (var entry in GameEntries)
        {
            PlayerPrefs.DeleteKey(entry.key);
        }
        PlayerPrefs.Save();
    }
}
