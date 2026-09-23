using UnityEngine;
using System.Diagnostics;

public static class PalmDataManager
{
    // 右手
    public static Vector3 RightPalm = Vector3.zero;
    public static bool RightGrabbing = false;

    // 左手
    public static Vector3 LeftPalm = Vector3.zero;
    public static bool LeftGrabbing = false;

    private static long rightLastUpdatedTicks;
    private static long leftLastUpdatedTicks;
    private static readonly long trackingTimeoutTicks =
        (long)(Stopwatch.Frequency * 0.25d);

    public static bool RightTracked => IsRecentlyUpdated(rightLastUpdatedTicks);
    public static bool LeftTracked => IsRecentlyUpdated(leftLastUpdatedTicks);

    public static void SetRightHand(Vector3 position, bool grabbing)
    {
        RightPalm = position;
        RightGrabbing = grabbing;
        rightLastUpdatedTicks = Stopwatch.GetTimestamp();
    }

    public static void SetLeftHand(Vector3 position, bool grabbing)
    {
        LeftPalm = position;
        LeftGrabbing = grabbing;
        leftLastUpdatedTicks = Stopwatch.GetTimestamp();
    }

    private static bool IsRecentlyUpdated(long lastUpdatedTicks)
    {
        return lastUpdatedTicks > 0 &&
            Stopwatch.GetTimestamp() - lastUpdatedTicks <= trackingTimeoutTicks;
    }

    // 受信用ヘルパー関数（必要に応じて使える）
    public static Vector3 ConvertNormalizedToWorld(float normX, float normY, float scale = 500f)
    {
        // XもZも反転して「画面通りの動き」に変換
        return new Vector3((1.0f - normX) * scale, 0, (1.0f - normY) * scale);
    }

public static Vector3 ConvertNormalizedToCanvas(Vector2 normalizedPos, RectTransform canvasRect)
{
    Vector2 size = canvasRect.sizeDelta;

    // 左右反転：xは0.5f - x に変更
    float x = (0.5f - normalizedPos.x) * size.x;
    float y = (1f - normalizedPos.y - 0.5f) * size.y; // 上下反転はそのままでOK

    return canvasRect.TransformPoint(new Vector3(x, y, 0));
}






}
