using UnityEngine;

public class AppIconStabilizer : MonoBehaviour
{
    [SerializeField] private Transform cameraTransform; // カメラ参照
    [SerializeField] private float wobbleSpeed = 2f;
    [SerializeField] private float wobbleAmount = 1f;

    private float wobbleOffset;
    private Quaternion baseRotation;

    void Start()
    {
        if (cameraTransform == null)
            cameraTransform = Camera.main.transform;

        // 揺れのランダムオフセット
        wobbleOffset = Random.Range(0f, Mathf.PI * 2f);

        // 初期のローカル回転（リング内での向き）
        baseRotation = transform.localRotation;
    }

    void Update()
    {
        // 揺れの演出（Y軸を中心に軽く回転）
        float wobble = Mathf.Sin(Time.time * wobbleSpeed + wobbleOffset) * wobbleAmount;
        Quaternion wobbleRotation = Quaternion.Euler(0f, wobble, 0f);

        // 親のZ軸回転だけを打ち消す
        float parentZ = transform.parent.eulerAngles.z;
        Quaternion counterZ = Quaternion.Euler(0f, 0f, -parentZ);

        // 初期姿勢 × 揺れ × Z打ち消し
        transform.localRotation = baseRotation * wobbleRotation * counterZ;
    }
}
