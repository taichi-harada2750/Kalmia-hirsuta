using UnityEngine;

public class AppIconStabilizer : MonoBehaviour
{
    [SerializeField] private Transform parentRing; // リングのTransform
    [SerializeField] private float maxSwing = 15f;
    [SerializeField] private float smoothTime = 0.1f;
    [SerializeField] private float wobbleSpeed = 2f;
    [SerializeField] private float wobbleAmount = 1f;

    private float previousZ;
    private float swingVelocity;
    private float currentSwing;
    private float wobbleOffset;
    private Quaternion baseRotation;

    void Start()
    {
        if (parentRing == null)
            parentRing = transform.parent;

        previousZ = parentRing.eulerAngles.z;
        wobbleOffset = Random.Range(0f, Mathf.PI * 2f);
        baseRotation = transform.localRotation;
    }

    void Update()
    {
        // --------- 親の回転差から振り子揺れを生成 ---------
        float currentZ = parentRing.eulerAngles.z;
        float deltaZ = Mathf.DeltaAngle(previousZ, currentZ);
        previousZ = currentZ;

        float targetSwing = Mathf.Clamp(-deltaZ * 3f, -maxSwing, maxSwing); // 振れ幅強調
        currentSwing = Mathf.SmoothDamp(currentSwing, targetSwing, ref swingVelocity, smoothTime);

        // --------- 微細なY軸揺れ（視覚演出） ---------
        float wobble = Mathf.Sin(Time.time * wobbleSpeed + wobbleOffset) * wobbleAmount;
        Quaternion wobbleRotation = Quaternion.Euler(0f, wobble, 0f);

        // --------- Z回転打ち消し（常に正面） + 振り子（X軸） ---------
        float parentZ = parentRing.eulerAngles.z;
        Quaternion cancelZ = Quaternion.Euler(0f, 0f, -parentZ);
        Quaternion swing = Quaternion.Euler(currentSwing, 0f, 0f);

        // --------- 合成して適用 ---------
        transform.localRotation = baseRotation * wobbleRotation * swing * cancelZ;
    }
}
