using UnityEngine;

public class SummonTrigger : MonoBehaviour
{
    public GameObject ringUI;

    private bool isHandInside = false;

    void OnTriggerEnter(Collider other)
    {
        if (other.name.Contains("Right")) // ← 表示上「右手」に見える手
        {
            isHandInside = true;
            Debug.Log("🟠 右手オブジェクトがTriggerに入りました（中身はLeftHand）");
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.name.Contains("Right"))
        {
            isHandInside = false;
            Debug.Log("⚪ 右手オブジェクトがTriggerから出ました");
        }
    }

    void Update()
    {
        if (isHandInside && PalmDataManager.LeftGrabbing) // ← 判定はLeftGrabbingに修正！
        {
            ringUI.SetActive(true);
            ringUI.transform.position = transform.position;

            // アニメーションでアイコンを展開
            var animator = ringUI.GetComponent<RingUIAnimator_RectTransform>();
            if (animator != null)
            {
                animator.PlaySummonAnimation();
            }

            Debug.Log("🟢 接触中 + LeftGrabbing でリングUI表示！");
            isHandInside = false;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            ringUI.SetActive(true);
            ringUI.transform.position = transform.position;

            // スペースキーでもアニメーション発火
            var animator = ringUI.GetComponent<RingUIAnimator_RectTransform>();
            if (animator != null)
            {
                animator.PlaySummonAnimation();
            }

            Debug.Log("🔵 スペースキーでリングUI表示！");
        }
    }
}
