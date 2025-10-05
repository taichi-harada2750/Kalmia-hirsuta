using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;
using System.Collections.Generic;  


public class DualAimController : MonoBehaviour
{
    [Header("UI")]
    public GameObject startButton;
    public TMP_Text countdownText;
    public TMP_Text timerText;
    public TMP_Text resultText;
    public TMP_Text scoreText;

    [Header("ゲーム設定")]
    public float gameTime = 30f;
    public Transform spawnArea;
    public int spawnCount = 30;

    private float timeRemaining;
    private bool isGameRunning = false;
    private bool hasRefilledTargets = false;
    private bool hasRefilledAfterGoodLow = false;

    [Header("プレハブ")]
    public GameObject goodTargetPrefab;
    public GameObject badTargetPrefab;
    public GameObject hardBadTargetPrefab;

    [Header("UI連携")]
    public CloseButton closeButton;

    public TargetResetter resetter;

    public ScoreDisplay scoreDisplay; // ← インスペクタで割り当て


    void Start()
    {
        countdownText.gameObject.SetActive(false);
        timerText.gameObject.SetActive(false);
        resultText.gameObject.SetActive(false);
        startButton.SetActive(true);
        scoreDisplay.gameObject.SetActive(true);


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

            int totalCount = GameObject.FindGameObjectsWithTag("Target").Length;
            int goodCount = CountGoodTargets();

            if (!hasRefilledTargets && totalCount < 10)
            {
                Debug.Log("🔵 全体ターゲット補充");
                SpawnTargets(); // 通常
                hasRefilledTargets = true;
            }
            else if (hasRefilledTargets && !hasRefilledAfterGoodLow && goodCount <= 5)
            {
                Debug.Log("🟢 Good枯渇後のハードターゲット補充");
                HardScoreHandler.Instance.hardModeActive = true; // Hardモード突入時（2回目のSpawn直前など）
                SpawnTargets(isHardMode: true); // ← 難易度上げた補充
                hasRefilledAfterGoodLow = true;
            }
        }
    }
    int CountGoodTargets()
    {
        int count = 0;
        var targets = GameObject.FindGameObjectsWithTag("Target");
        foreach (var t in targets)
        {
            var g = t.GetComponent<GrabbableDestroyer>();
            if (g != null && g.type == GrabbableDestroyer.ObjectType.Good)
                count++;
        }
        return count;
    }

    public void OnStartButtonPressed()
    {
        // UIと状態を初期化
        resultText.gameObject.SetActive(false);
        timerText.text = "";
        timerText.gameObject.SetActive(false);
        if (scoreText != null) scoreText.text = "Score: 0";
        SortGameManager.Instance.ResetScore();

        // 既存ターゲット削除
        ClearExistingTargets();

        hasRefilledTargets = false;
        startButton.SetActive(false);
        scoreDisplay.gameObject.SetActive(false);
        StartCoroutine(GameSequence());
    }

    IEnumerator GameSequence()
    {
        countdownText.gameObject.SetActive(true);
        string[] countdown = { "3", "2", "1", "Start!" };
        foreach (string num in countdown)
        {
            countdownText.text = num;
            yield return new WaitForSeconds(1f);
        }
        countdownText.gameObject.SetActive(false);

        closeButton?.Block();

        if (scoreText != null) scoreText.gameObject.SetActive(true);

        SpawnTargets();

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

        resultText.text = $"SCORE: {SortGameManager.Instance.GetScore()}";
        resultText.gameObject.SetActive(true);

        ClearExistingTargets();
        EndGame();
        closeButton?.Allow();

        SortGameManager.Instance.ResetScore();
        if (scoreText != null) scoreText.text = "Score: 0";

        startButton.SetActive(true);
        scoreDisplay.gameObject.SetActive(true);
    }

    void SpawnTargets(bool isHardMode = false)
    {
        for (int i = 0; i < spawnCount; i++)
        {
            GameObject prefab;

            if (isHardMode)
            {
                // ⚡ Hardモード：HardBad と Good のみ出現
                prefab = (Random.value < 0.4f)
                    ? hardBadTargetPrefab   // Hard専用敵（速度・挙動はPrefab側で設定）
                    : goodTargetPrefab;
            }
            else
            {
                // 通常モード：Good多め、Bad少なめ
                bool isBad = Random.value < 0.2f;
                prefab = isBad ? badTargetPrefab : goodTargetPrefab;
            }

            GameObject obj = Instantiate(prefab, GetRandomPosition(), Quaternion.identity);
            obj.tag = "Target";
            obj.layer = LayerMask.NameToLayer("Target");

            // --- タイプ設定 ---
            if (obj.GetComponent<GrabbableDestroyer>() == null)
            {
                var grab = obj.AddComponent<GrabbableDestroyer>();
                grab.type = (prefab == badTargetPrefab || prefab == hardBadTargetPrefab)
                    ? GrabbableDestroyer.ObjectType.Bad
                    : GrabbableDestroyer.ObjectType.Good;
            }
        }
    }


    Vector3 GetRandomPosition()
    {
        Vector3 center = new Vector3(5.1f, 65f, 27.9f);
        float width = 450f;
        float height = 260f;

        float x = Random.Range(center.x - width / 2f, center.x + width / 2f);
        float y = Random.Range(center.y - height / 2f, center.y + height / 2f);
        float z = center.z;

        return new Vector3(x, y, z);
    }

    void OnDrawGizmos()
    {
        Vector3 center = new Vector3(5.1f, -2f, 27.9f);
        Vector3 size = new Vector3(45f, 26f, 0.1f);
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(center, size);
    }

void EndGame()
{
    int finalScore = SortGameManager.Instance.GetScore();
    ScoreManager.SaveScore(finalScore);

    // 🔹 即時ランキング更新
    if (scoreDisplay != null)
        scoreDisplay.UpdateRanking();

    resetter?.ResetAll();
}



    void ClearExistingTargets()
    {
        var targets = GameObject.FindGameObjectsWithTag("Target");
        foreach (var t in targets)
        {
            Destroy(t);
        }
    }
}
