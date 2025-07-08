using UnityEngine;

public class ResetRingUI : MonoBehaviour
{
    public GameObject ringUI;
    public GameObject summonButton;
    public GameObject resetButton;
    public SummonTrigger summonTrigger;

    private float inputBlockTimer = 0f;
    public float inputBlockDuration = 0.6f;

    void Update()
    {
        if (inputBlockTimer > 0f)
        {
            inputBlockTimer -= Time.deltaTime;
            return;
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            ResetUI();
        }
    }

    public void ResetUI()
    {
        Debug.Log("🔁 ResetUI() 実行");

        if (ringUI != null)
        {
            ringUI.SetActive(false);
            Debug.Log("🔴 ringUI を非表示にしました");
        }

        if (resetButton != null)
        {
            resetButton.SetActive(false);
            Debug.Log("🟡 resetButton を非表示にしました");
        }

        if (summonButton != null)
        {
            summonButton.SetActive(true);
            Debug.Log("🟢 summonButton を再表示しました");
        }

        // 自身のグラブ入力も一時ブロック
        inputBlockTimer = inputBlockDuration;

        // SummonTrigger 側のブロックも呼び出し
        if (summonTrigger != null)
        {
            summonTrigger.BlockInputForSeconds(inputBlockDuration);
            Debug.Log("⏱️ summonTrigger に入力ブロックを要求しました");
        }
    }
}
