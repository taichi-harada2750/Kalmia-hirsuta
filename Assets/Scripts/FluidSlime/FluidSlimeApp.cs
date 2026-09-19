using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;

namespace FluidSlime
{
    public class FluidSlimeApp : MonoBehaviour
    {
        [Header("UI")]
        public GameObject startButton;
        public TMP_Text countdownText;
        public TMP_Text timerText;
        public TMP_Text resultText;
        public TMP_Text scoreText;
        public TMP_Text massText; // 現在の質量表示用

        [Header("ゲーム設定")]
        public float gameTime = 30f;
        public Transform spawnArea;
        public int initialSpawnCount = 2;
        public float spawnInterval = 1.5f;

        [Header("プレハブ")]
        public GameObject targetPrefab;

        private float timeRemaining;
        public bool IsGameRunning { get; private set; } = false;
        private Coroutine spawnCoroutine;
        private bool hasStartedApp = false;

        public static FluidSlimeApp Instance { get; private set; }

        void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);
        }

        void Start()
        {
            if (!hasStartedApp)
            {
                StartApp();
            }
        }

        public void ExtendTimer(float amount)
        {
            if (IsGameRunning)
            {
                timeRemaining += amount;
            }
        }

        public void StartApp()
        {
            hasStartedApp = true;
            countdownText.gameObject.SetActive(false);
            timerText.gameObject.SetActive(false);
            resultText.gameObject.SetActive(false);
            startButton.SetActive(true);
            
            if (SortGameManager.Instance != null)
            {
                SortGameManager.Instance.ResetScore();
            }
            if (scoreText != null) scoreText.text = "Score: 0";
            if (massText != null) massText.text = "Mass: -";
        }

        void Update()
        {
            if (IsGameRunning)
            {
                if (scoreText != null && SortGameManager.Instance != null)
                {
                    scoreText.text = $"Score: {SortGameManager.Instance.GetScore()}";
                }
                if (massText != null && SlimeController.Instance != null)
                {
                    massText.text = $"Mass: {SlimeController.Instance.totalMass:F1}";
                }
            }
        }

        public void OnStartButtonPressed()
        {
            resultText.gameObject.SetActive(false);
            timerText.gameObject.SetActive(false);
            if (scoreText != null) scoreText.text = "Score: 0";
            
            if (SortGameManager.Instance != null)
            {
                SortGameManager.Instance.ResetScore();
            }

            // スライムのステータスを初期化
            if (SlimeController.Instance != null)
            {
                SlimeController.Instance.ResetSlime(); 
            }

            ClearExistingTargets();

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

            // 初期スポーン
            for (int i = 0; i < initialSpawnCount; i++)
            {
                SpawnTarget();
            }

            timeRemaining = gameTime;
            IsGameRunning = true;
            timerText.gameObject.SetActive(true);

            spawnCoroutine = StartCoroutine(SpawnRoutine());

            while (timeRemaining > 0)
            {
                timeRemaining -= Time.deltaTime;
                timerText.text = $"Time: {Mathf.CeilToInt(timeRemaining)}";

                // もしスライムが死んだら（質量が一定以下になったら）ゲームオーバーにすることも可能
                if (SlimeController.Instance != null && SlimeController.Instance.totalMass <= 0.5f)
                {
                    break;
                }

                yield return null;
            }

            IsGameRunning = false;
            timerText.gameObject.SetActive(false);
            if (spawnCoroutine != null) StopCoroutine(spawnCoroutine);

            // リザルト
            if (SortGameManager.Instance != null)
            {
                resultText.text = $"SCORE: {SortGameManager.Instance.GetScore()}";
            }
            else
            {
                resultText.text = "GAME OVER";
            }
            resultText.gameObject.SetActive(true);

            ClearExistingTargets();
            EndGame();

            startButton.SetActive(true);
        }

        IEnumerator SpawnRoutine()
        {
            while (IsGameRunning)
            {
                yield return new WaitForSeconds(spawnInterval);
                int currentTargets = GameObject.FindGameObjectsWithTag("Target").Length;
                if (currentTargets < 5) // 同時出現数を5までに制限（自然消滅もするため少し多めでOK）
                {
                    float r = Random.value;
                    if (r < 0.2f)
                    {
                        // 巨大敵 (20%)
                        SpawnTarget(TargetType.Giant);
                    }
                    else if (r < 0.4f)
                    {
                        // 群れ (20%)
                        int swarmCount = Random.Range(3, 6);
                        for (int i = 0; i < swarmCount; i++)
                        {
                            SpawnTarget(TargetType.Swarm);
                        }
                    }
                    else
                    {
                        // 通常 (60%)
                        SpawnTarget(TargetType.Normal);
                    }
                }
            }
        }

        enum TargetType { Normal, Swarm, Giant }

        void SpawnTarget(TargetType type = TargetType.Normal)
        {
            Vector3 pos = GetRandomPosition();
            GameObject obj = Instantiate(targetPrefab, pos, Quaternion.identity);
            obj.tag = "Target";

            SlimeTarget st = obj.GetComponent<SlimeTarget>();
            if (st == null) st = obj.AddComponent<SlimeTarget>();

            float currentSlimeMass = SlimeController.Instance != null ? SlimeController.Instance.totalMass : 2.0f;
            
            switch (type)
            {
                case TargetType.Giant:
                    // 少しだけ大きい敵（逃げ切れる・成長すればすぐ勝てるサイズ）
                    st.mass = currentSlimeMass * Random.Range(1.2f, 1.5f);
                    break;
                case TargetType.Swarm:
                    // 非常に小さい群れ
                    st.mass = Mathf.Max(0.5f, currentSlimeMass * Random.Range(0.05f, 0.15f));
                    break;
                case TargetType.Normal:
                default:
                    // 確実に食べられる小さい敵
                    st.mass = Mathf.Max(0.5f, currentSlimeMass * Random.Range(0.2f, 0.4f));
                    break;
            }
        }

        Vector3 GetRandomPosition()
        {
            // DualAimShootingの範囲を参考
            Vector3 center = new Vector3(5.1f, 65f, 27.9f); // 画面上部から落ちてくるなどの演出に合わせた座標
            float width = 450f;
            float height = 260f;

            float x = Random.Range(center.x - width / 2f, center.x + width / 2f);
            float y = Random.Range(center.y - height / 2f, center.y + height / 2f);
            float z = center.z;

            return new Vector3(x, y, z);
        }

        void OnDrawGizmos()
        {
            Vector3 center = new Vector3(5.1f, 65f, 27.9f);
            Vector3 size = new Vector3(450f, 260f, 0.1f);
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireCube(center, size);
        }

        void EndGame()
        {
            int finalScore = SortGameManager.Instance != null ? SortGameManager.Instance.GetScore() : 0;
            ScoreManager.SaveScore(finalScore, "FluidSlimeScores");
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
}
