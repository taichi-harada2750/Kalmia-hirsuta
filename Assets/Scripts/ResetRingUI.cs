using UnityEngine;

public class ResetRingUI : MonoBehaviour
{
    public GameObject ringUI;       // 展開されるリングUI
    public GameObject summonButton; // 元の召喚ボタン
    public GameObject resetButton;  // このリセットボタン自身

    public void ResetUI()
    {
        // リングUIを非表示に
        if (ringUI != null)
        {
            ringUI.SetActive(false);
            Debug.Log("🔴 リングUIを非表示にしました");
        }

        // ResetButtonを非表示に、SummonButtonを再表示
        if (resetButton != null)
        {
            resetButton.SetActive(false);
            Debug.Log("🟡 Resetボタンを非表示にしました");
        }

        if (summonButton != null)
        {
            summonButton.SetActive(true);
            Debug.Log("🟢 Summonボタンを再表示しました");
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            ResetUI();
        }
    }
}
