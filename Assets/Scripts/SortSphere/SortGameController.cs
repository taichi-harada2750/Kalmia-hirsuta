using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class SortGameController : MonoBehaviour
{
    [Header("UI")]
    public GameObject startButton;
    public TMP_Text countdownText;
    public TMP_Text timerText;
    public TMP_Text resultText;
    public TMP_Text scoreText;


    [Header("ゲーム設定")]
    public float gameTime = 30f;
    public GameObject spherePrefab;
    public GameObject cubePrefab;
    public Transform spawnArea;
    public int spawnCount = 30;

    private float timeRemaining;
    private bool isGameRunning = false;

    void Start()
    {
        countdownText.gameObject.SetActive(false);
        timerText.gameObject.SetActive(false);
        resultText.gameObject.SetActive(false);
        startButton.SetActive(true);

        SortGameManager.Instance.ResetScore();
        if (scoreText != null)
        {
            scoreText.text = "Score: 0";
        }
    }
    void Update()
    {
        if (isGameRunning && scoreText != null)
        {
            scoreText.text = $"Score: {SortGameManager.Instance.GetScore()}";
        }
    }


    public void OnStartButtonPressed()
    {
        startButton.SetActive(false);
        StartCoroutine(GameSequence());
    }

    IEnumerator GameSequence()
    {
        // カウントダウン演出
        countdownText.gameObject.SetActive(true);
        string[] countdown = { "3", "2", "1", "Start!" };
        foreach (string num in countdown)
        {
            countdownText.text = num;
            yield return new WaitForSeconds(1f);
        }
        countdownText.gameObject.SetActive(false);

        if (scoreText != null) scoreText.gameObject.SetActive(true);

        // オブジェクト生成
        SpawnObjects();

        // タイマースタート
        timeRemaining = gameTime;
        isGameRunning = true;
        timerText.gameObject.SetActive(true);

        while (timeRemaining > 0)
        {
            timeRemaining -= Time.deltaTime;
            timerText.text = $"Time: {Mathf.CeilToInt(timeRemaining)}";
            yield return null;
        }

        isGameRunning = false;
        timerText.gameObject.SetActive(false);

        // スコア表示（SortGameManagerから取得）
        resultText.text = $"SCORE: {SortGameManager.Instance.GetScore()}";
        resultText.gameObject.SetActive(true);
    }

    void SpawnObjects()
    {
        for (int i = 0; i < spawnCount; i++)
        {
            Instantiate(spherePrefab, GetRandomPosition(), Quaternion.identity);
            Instantiate(cubePrefab, GetRandomPosition(), Quaternion.identity);
        }
    }

    Vector3 GetRandomPosition()
    {
        Vector3 center = new Vector3(-11f, -146f, 27.9f);
        float width = 24.752f * 10f;
        float height = 16.644f * 10f;

        float x = Random.Range(center.x - width / 2f, center.x + width / 2f);
        float y = Random.Range(center.y - height / 2f, center.y + height / 2f);
        float z = center.z + 1.0f;

        return new Vector3(x, y, z);
    }
}
