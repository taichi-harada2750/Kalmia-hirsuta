using UnityEngine;
using extOSC;

// 実行順を最優先化。消費側スクリプトより先にPalmDataManagerを更新することで、
// 「受信が消費より後に走って1フレーム古い値を読む」遅延を防ぐ。
[DefaultExecutionOrder(-100)]
public class OSCHandReceiver : MonoBehaviour
{
    public OSCReceiver receiver;
    public string leftAddress = "/hand/left_palm";
    public string rightAddress = "/hand/right_palm";
    public float scale = 500f;
    public RectTransform canvasRect; // CanvasのRectTransformをインスペクターで指定

    [Header("One-Euro Filter (ジッター/ラグ調整)")]
    [Tooltip("小さいほど静止時のジッターが減る（動き出しの遅延は微増）。0.5〜2.0程度で調整")]
    public float minCutoff = 1.0f;
    [Tooltip("大きいほど速い動きの追従が良くなる（速い時ほど遅延が減る）。0.0〜0.05程度で調整")]
    public float beta = 0.007f;

    [Header("予測 (30fps固定のラグ相殺)")]
    [Tooltip("速度から何秒先の位置を予測するか。33fps=0.033が1フレーム分。0.03〜0.07で調整。大きすぎると行き過ぎ(オーバーシュート)する")]
    public float predictionTime = 0.04f;
    [Tooltip("予測による1軸の最大移動量(正規化座標)。手の再検出で速度が跳ねた際の暴走を防ぐ")]
    public float maxPredictDelta = 0.15f;

    [Header("ワープ/復帰対策")]
    [Tooltip("この秒数以上更新が途切れたら『手を見失って再検出した』とみなし、フィルタをリセットして瞬時にスナップ（予測による飛びを防ぐ）")]
    public float lostTimeout = 0.2f;
    [Tooltip("前回位置からの正規化座標のジャンプがこの値を超えたらテレポート扱いでスナップ。0.25〜0.4程度。左右取り違えや誤検出の飛びを吸収")]
    public float jumpThreshold = 0.3f;

    // 正規化座標(0〜1)に対して平滑化してから座標変換する。
    // 受信直後にかけることで、PalmDataManagerを読む全ての消費者が恩恵を受ける。
    private readonly HandState left = new HandState();
    private readonly HandState right = new HandState();

    // 1つの手の受信状態
    private class HandState
    {
        public readonly OneEuroFilterVector2 filter = new OneEuroFilterVector2();
        public float lastTime = -1f;
        public Vector2 lastRaw;
        public bool hasRaw;
    }

    void Start()
    {
        receiver.Bind(leftAddress, OnLeftPalmReceived);
        receiver.Bind(rightAddress, OnRightPalmReceived);
    }

    void OnRightPalmReceived(OSCMessage message)
    {
        if (ProcessMessage(message, right, out Vector2 pos, out bool grab))
        {
            PalmDataManager.SetRightHand(
                PalmDataManager.ConvertNormalizedToCanvas(pos, canvasRect), grab);
        }
    }

    void OnLeftPalmReceived(OSCMessage message)
    {
        if (ProcessMessage(message, left, out Vector2 pos, out bool grab))
        {
            PalmDataManager.SetLeftHand(
                PalmDataManager.ConvertNormalizedToCanvas(pos, canvasRect), grab);
        }
    }

    // 生の正規化座標を平滑化＋予測し、必要ならスナップして返す。
    private bool ProcessMessage(OSCMessage message, HandState state, out Vector2 filtered, out bool grab)
    {
        filtered = Vector2.zero;
        grab = false;
        if (message.Values.Count < 3) return false;

        Vector2 raw = new Vector2(message.Values[0].FloatValue, message.Values[1].FloatValue);
        grab = message.Values[2].FloatValue > 0.5f;

        float dt = (state.lastTime < 0f) ? 0f : Time.realtimeSinceStartup - state.lastTime;
        state.lastTime = Time.realtimeSinceStartup;

        // 見失い後の復帰（長い空白）や、前回位置からの大ジャンプ（左右取り違え・誤検出）は
        // 予測やフィルタの慣性で画面を横切るように飛ぶ。フィルタをリセットして瞬時にスナップさせる。
        bool reacquired = dt > lostTimeout;
        bool teleport = state.hasRaw && Vector2.Distance(raw, state.lastRaw) > jumpThreshold;
        if (reacquired || teleport)
        {
            state.filter.Reset();
            dt = 0f; // リセット直後は予測を無効化（速度0でスナップ）
        }

        state.filter.SetParams(minCutoff, beta);
        filtered = state.filter.FilterPredicted(raw, dt, predictionTime, maxPredictDelta);

        state.lastRaw = raw;
        state.hasRaw = true;
        return true;
    }
}
