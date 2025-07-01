using UnityEngine;

public static class PalmDataManager
{
    // 右手
    public static Vector3 RightPalm = Vector3.zero;
    public static bool RightGrabbing = false;

    // 左手
    public static Vector3 LeftPalm = Vector3.zero;
    public static bool LeftGrabbing = false;

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
