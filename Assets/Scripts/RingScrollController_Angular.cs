using UnityEngine;

public class RingScrollController_Angular : MonoBehaviour
{
    public Transform ringUI;                  // 回転させるUI
    public float rotationMultiplier = 1.2f;   // 回転のスケーリング
    public float damping = 6f;                // 慣性の減衰スピード（大きいほど早く止まる）
    public float angleThreshold = 1.5f;       // デッドゾーン（角度が小さいと無視）

    [Header("スクロール効果音キー（SoundManagerに登録済みのキー）")]
    public string scrollSEKey = "scroll";

    private bool isGrabbing = false;
    private Vector3 center;
    private Vector3 lastHandPos;
    private float currentVelocity = 0f;

    void Update()
    {
        Vector3 handPos = PalmDataManager.LeftPalm;
        bool grabbing = PalmDataManager.LeftGrabbing;

        // Grab開始
        if (!isGrabbing && grabbing)
        {
            isGrabbing = true;
            center = ringUI.position;
            lastHandPos = handPos;

            // スクロール効果音を鳴らす
            if (SoundManager.Instance != null && !string.IsNullOrEmpty(scrollSEKey))
            {
                SoundManager.Instance.PlaySE(scrollSEKey);
            }
        }

        // Grab中：角度差分で回転
        if (isGrabbing && grabbing)
        {
            Vector2 from = lastHandPos - center;
            Vector2 to = handPos - center;

            float angleDelta = Vector2.SignedAngle(from, to);

            // 小さなぶれは無視
            if (Mathf.Abs(angleDelta) > angleThreshold)
            {
                float appliedRotation = angleDelta * rotationMultiplier;
                ringUI.Rotate(0f, 0f, appliedRotation);
                currentVelocity = appliedRotation; // 慣性へ受け渡し
            }

            lastHandPos = handPos;
        }

        // 離したあと：慣性で回転継続
        if (!grabbing && isGrabbing)
        {
            ringUI.Rotate(0f, 0f, currentVelocity * Time.deltaTime);
            currentVelocity = Mathf.Lerp(currentVelocity, 0f, Time.deltaTime * damping);

            // 小さくなったら停止
            if (Mathf.Abs(currentVelocity) < 0.1f)
            {
                currentVelocity = 0f;
                isGrabbing = false;
            }
        }
    }
}
