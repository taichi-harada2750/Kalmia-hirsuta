using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

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


    [Header("プレハブ")]
    public GameObject goodTargetPrefab;
    public GameObject badTargetPrefab;


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
        countdownText.gameObject.SetActive(true);
        string[] countdown = { "3", "2", "1", "Start!" };
        foreach (string num in countdown)
        {
            countdownText.text = num;
            yield return new WaitForSeconds(1f);
        }
        countdownText.gameObject.SetActive(false);

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
    }

    void SpawnTargets()
    {
        for (int i = 0; i < spawnCount; i++)
        {
            bool isBad = Random.value < 0.2f;

            GameObject prefab = isBad ? badTargetPrefab : goodTargetPrefab;
            GameObject obj = Instantiate(prefab, GetRandomPosition(), Quaternion.identity);

            obj.tag = "Target";
            obj.layer = LayerMask.NameToLayer("Target");

            // もし TargetObject や Destroyer がプレハブに付いていないなら動的に付加
            if (obj.GetComponent<TargetObject>() == null)
                obj.AddComponent<TargetObject>();

            if (obj.GetComponent<GrabbableDestroyer>() == null)
            {
                var grab = obj.AddComponent<GrabbableDestroyer>();
                grab.type = isBad ? GrabbableDestroyer.ObjectType.Bad : GrabbableDestroyer.ObjectType.Good;
            }
        }
    }



    Vector3 GetRandomPosition()
    {
        // 中心位置はそのまま（Planeの位置）
        Vector3 center = new Vector3(5.1f, 65f, 27.9f);

        // 元の見た目ベース：幅45、高さ26 → それを10倍
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

    public TargetResetter resetter;

    void EndGame()
    {
        // ゲーム結果処理などの後で
        resetter?.ResetAll();
    }


    




}
