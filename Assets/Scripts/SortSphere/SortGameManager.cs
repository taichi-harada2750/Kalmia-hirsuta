using UnityEngine;
using TMPro;

public class SortGameManager : MonoBehaviour
{
    public static SortGameManager Instance;

    public int score = 0;
    public TMP_Text scoreText;

    // --- ここを追加 ---
    public delegate void ScoreChangedHandler(bool isCorrect);
    public static event ScoreChangedHandler OnScoreChanged;
    // -------------------

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void ResetScore()
    {
        score = 0;
        UpdateScoreText();
    }

    public void AddScore(bool correct)
    {
        score += correct ? 1 : -1;

        // --- スコアが変わったことを通知 ---
        OnScoreChanged?.Invoke(correct);

        UpdateScoreText();
    }

    private void UpdateScoreText()
    {
        if (scoreText != null)
        {
            scoreText.text = $"Score: {score}";
        }
    }

    public int GetScore()
    {
        return score;
    }
}
