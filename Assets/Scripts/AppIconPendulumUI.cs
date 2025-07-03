using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class AppIconPendulumUI : MonoBehaviour
{
    [SerializeField] private Transform parentRing;       // リング本体
    [SerializeField] private float maxSwingAngle = 15f;  // 最大揺れ角（度）
    [SerializeField] private float smoothTime = 0.15f;   // スムージング時間

    private RectTransform rectTransform;
    private float previousZ;
    private float swingVelocity;
    private float currentSwing;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        if (parentRing == null)
            parentRing = transform.parent;

        previousZ = parentRing.eulerAngles.z;
    }

    void Update()
    {
        float currentZ = parentRing.eulerAngles.z;
        float deltaZ = Mathf.DeltaAngle(previousZ, currentZ);
        previousZ = currentZ;

        // 振り子のターゲット角度（反対方向に揺れる）
        float targetSwing = Mathf.Clamp(-deltaZ * 2f, -maxSwingAngle, maxSwingAngle);
        currentSwing = Mathf.SmoothDamp(currentSwing, targetSwing, ref swingVelocity, smoothTime);

        // Z軸キャンセル + 揺れ（ZはUIの回転、X/Yではない）
        float parentZ = parentRing.eulerAngles.z;
        Quaternion baseCancel = Quaternion.Euler(0f, 0f, -parentZ);
        Quaternion swing = Quaternion.Euler(0f, 0f, currentSwing); // Z軸回転に揺れを足す

        rectTransform.localRotation = baseCancel * swing;
    }
}
