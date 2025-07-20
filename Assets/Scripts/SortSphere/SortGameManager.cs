using UnityEngine;
using TMPro;

public class SortGameManager : MonoBehaviour
{
    public static SortGameManager Instance;

    public int score = 0;
    public TMP_Text scoreText;

    void Awake()
    {
        // シングルトン化
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
