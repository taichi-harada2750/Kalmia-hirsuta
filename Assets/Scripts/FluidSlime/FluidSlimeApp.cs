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
        public float gameTime = 60f;
        public Transform spawnArea;
        public int initialSpawnCount = 10;
        public float spawnInterval = 3f;

        [Header("プレハブ")]
        public GameObject targetPrefab;

        private float timeRemaining;
        private bool isGameRunning = false;
        private Coroutine spawnCoroutine;
        private bool hasStartedApp = false;

        void Start()
        {
            if (!hasStartedApp)
            {
                StartApp();
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
            if (isGameRunning)
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

            // スライムの質量を初期化
            if (SlimeController.Instance != null)
            {
                SlimeController.Instance.totalMass = 2.0f; // 初期サイズ
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
            isGameRunning = true;
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

            isGameRunning = false;
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
            while (isGameRunning)
            {
                yield return new WaitForSeconds(spawnInterval);
                int currentTargets = GameObject.FindGameObjectsWithTag("Target").Length;
                if (currentTargets < 20) // 最大数制限
                {
                    SpawnTarget();
                }
            }
        }

        void SpawnTarget()
        {
            Vector3 pos = GetRandomPosition();
            GameObject obj = Instantiate(targetPrefab, pos, Quaternion.identity);
            obj.tag = "Target";

            SlimeTarget st = obj.GetComponent<SlimeTarget>();
            if (st == null) st = obj.AddComponent<SlimeTarget>();

            // ターゲットのサイズをランダムに設定（現在のスライムのサイズに応じて少し大きいものも混ぜる）
            float currentSlimeMass = SlimeController.Instance != null ? SlimeController.Instance.totalMass : 2.0f;
            
            // 70%の確率でスライムより小さく食べやすい、30%の確率でスライムより大きく危険
            bool isDangerous = Random.value < 0.3f;
            if (isDangerous)
            {
                st.mass = currentSlimeMass + Random.Range(0.5f, 2.0f);
            }
            else
            {
                st.mass = Mathf.Max(0.5f, currentSlimeMass * Random.Range(0.2f, 0.8f));
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
            ScoreManager.SaveScore(finalScore);
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
