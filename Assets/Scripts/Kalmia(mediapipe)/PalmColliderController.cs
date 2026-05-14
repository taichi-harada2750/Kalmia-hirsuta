using UnityEngine;

public class PalmColliderController : MonoBehaviour
{
    public enum HandType { Left, Right }
    public HandType handType = HandType.Right;
    private Collider col;

    void Start()
    {
        col = GetComponent<Collider>();
        col.enabled = true; // KISのイベント駆動システムでは常時ONにしてHoverなどを事前検知させます
        Debug.Log("カルミアシステムが起動しました。");
    }

    void Update()
    {
        // KISアーキテクチャでは意図（GrabIntent）が通知されるため、
        // 物理コライダーのON/OFFを用いた判定は不要になりました。
        // （これをON/OFFしていると、GrabしたフレームでPhysics側が追いつかずクリックが無視される原因になります）
    }
}
