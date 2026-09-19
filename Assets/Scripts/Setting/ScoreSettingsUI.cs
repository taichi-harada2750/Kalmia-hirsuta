using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

/// <summary>
/// 設定画面に配置するスコア管理パネル。
/// 各ゲームのランキング表示と、個別／全リセットボタンを提供する。
/// 
/// ■ セットアップ手順（Unity Editor）
/// 1. Setting パネル内に空の GameObject を作り、このスクリプトをアタッチ。
/// 2. scoreDisplayArea  → ランキングテキストを表示する TMP_Text を割り当て。
/// 3. buttonContainer   → リセットボタンを並べる親 Transform（Vertical Layout Group 推奨）を割り当て。
/// 4. resetButtonPrefab → ボタンのプレハブ（子に TMP_Text を持つ UI Button）を割り当て。
///    プレハブがなければ、Unity 標準の Button (TextMeshPro) を使用可能。
/// 5. resetAllButton    → 「全スコアリセット」ボタンを割り当て（任意）。
/// </summary>
public class ScoreSettingsUI : MonoBehaviour
{
    [Header("表示")]
    [Tooltip("ランキング一覧を表示するテキスト")]
    public TMP_Text scoreDisplayArea;

    [Header("リセットボタン")]
    [Tooltip("ゲームごとのリセットボタンを生成する親 Transform（Vertical Layout Group 推奨）")]
    public Transform buttonContainer;

    [Tooltip("リセットボタンのプレハブ（子に TMP_Text を持つ Button）")]
    public GameObject resetButtonPrefab;

    [Tooltip("全スコアリセットボタン（手動配置する場合はここに割り当て）")]
    public Button resetAllButton;

    void OnEnable()
    {
        RefreshAll();
    }

    void Start()
    {
        // 全リセットボタンのイベント登録
        if (resetAllButton != null)
        {
            resetAllButton.onClick.AddListener(OnResetAllClicked);
        }

        BuildResetButtons();
        RefreshScoreDisplay();
    }

    /// <summary>
    /// ゲームごとのリセットボタンを動的に生成する
    /// </summary>
    void BuildResetButtons()
    {
        if (buttonContainer == null || resetButtonPrefab == null) return;

        // 既存の動的ボタンをクリア
        foreach (Transform child in buttonContainer)
        {
            Destroy(child.gameObject);
        }

        var entries = ScoreManager.GetAllGameEntries();
        foreach (var entry in entries)
        {
            GameObject btnObj = Instantiate(resetButtonPrefab, buttonContainer);
            btnObj.name = $"ResetBtn_{entry.key}";

            // ボタンのテキストを設定
            TMP_Text label = btnObj.GetComponentInChildren<TMP_Text>();
            if (label != null)
            {
                label.text = $"{entry.displayName}\nスコアをリセット";
            }

            // クリックイベント登録
            Button btn = btnObj.GetComponent<Button>();
            if (btn != null)
            {
                string capturedKey = entry.key;
                string capturedName = entry.displayName;
                btn.onClick.AddListener(() => OnResetGameClicked(capturedKey, capturedName));
            }
        }
    }

    /// <summary>
    /// 全ゲームのランキングを表示テキストに反映する
    /// </summary>
    void RefreshScoreDisplay()
    {
        if (scoreDisplayArea == null) return;

        string display = "";
        var entries = ScoreManager.GetAllGameEntries();

        foreach (var entry in entries)
        {
            display += $"<b>■ {entry.displayName}</b>\n";

            List<int> scores = ScoreManager.LoadScores(entry.key);
            if (scores.Count == 0)
            {
                display += "   （スコアなし）\n";
            }
            else
            {
                for (int i = 0; i < scores.Count; i++)
                {
                    display += $"   {i + 1}. {scores[i]} pts\n";
                }
            }
            display += "\n";
        }

        scoreDisplayArea.text = display;
    }

    /// <summary>
    /// 特定ゲームのスコアをリセット
    /// </summary>
    void OnResetGameClicked(string gameKey, string displayName)
    {
        ScoreManager.ResetScores(gameKey);
        Debug.Log($"[ScoreSettingsUI] {displayName} のスコアをリセットしました。");
        RefreshScoreDisplay();
    }

    /// <summary>
    /// 全ゲームのスコアをリセット
    /// </summary>
    void OnResetAllClicked()
    {
        ScoreManager.ResetAllScores();
        Debug.Log("[ScoreSettingsUI] 全ゲームのスコアをリセットしました。");
        RefreshScoreDisplay();
    }

    /// <summary>
    /// 表示を更新する（外部から呼び出し可能）
    /// </summary>
    public void RefreshAll()
    {
        RefreshScoreDisplay();
    }
}
