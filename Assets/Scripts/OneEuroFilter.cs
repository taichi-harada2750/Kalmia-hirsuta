using UnityEngine;

/// <summary>
/// One-Euroフィルタ（1スカラー用）。
/// 低速時は強く平滑化してジッターを消し、高速時は追従を優先して遅延を抑える。
/// Lerpのような固定係数フィルタと違い「低ラグ×低ジッター」を両立できる。
/// 参考: Casiez et al., "1€ Filter" (CHI 2012)
/// </summary>
public class OneEuroFilter
{
    // --- チューニングパラメータ ---
    // MinCutoff: 小さいほど静止時のジッターが減るが、動き出しの遅延がわずかに増える。
    // Beta:      大きいほど速い動きへの追従が良くなる（＝速い時ほど遅延が減る）。
    public float MinCutoff;
    public float Beta;
    public float DCutoff;

    private float _xPrev;
    private float _dxPrev;
    private bool _initialized;

    /// <summary>直近の平滑化済み速度（単位/秒）。予測外挿に使う。</summary>
    public float Velocity => _dxPrev;

    public OneEuroFilter(float minCutoff = 1.0f, float beta = 0.007f, float dCutoff = 1.0f)
    {
        MinCutoff = minCutoff;
        Beta = beta;
        DCutoff = dCutoff;
    }

    private static float Alpha(float cutoff, float dt)
    {
        float tau = 1.0f / (2.0f * Mathf.PI * cutoff);
        return 1.0f / (1.0f + tau / dt);
    }

    /// <param name="x">生の入力値</param>
    /// <param name="dt">前回入力からの経過時間（秒）</param>
    public float Filter(float x, float dt)
    {
        if (!_initialized || dt <= 0f)
        {
            _xPrev = x;
            _dxPrev = 0f;
            _initialized = true;
            return x;
        }

        // 微分値を平滑化してカットオフ周波数を動的に決める
        float dx = (x - _xPrev) / dt;
        float aD = Alpha(DCutoff, dt);
        float dxHat = aD * dx + (1f - aD) * _dxPrev;

        float cutoff = MinCutoff + Beta * Mathf.Abs(dxHat);
        float a = Alpha(cutoff, dt);
        float xHat = a * x + (1f - a) * _xPrev;

        _xPrev = xHat;
        _dxPrev = dxHat;
        return xHat;
    }

    public void Reset()
    {
        _initialized = false;
    }
}

/// <summary>
/// Vector2用のOne-Euroフィルタ（各成分に独立適用）。
/// </summary>
public class OneEuroFilterVector2
{
    private readonly OneEuroFilter _x;
    private readonly OneEuroFilter _y;

    public OneEuroFilterVector2(float minCutoff = 1.0f, float beta = 0.007f, float dCutoff = 1.0f)
    {
        _x = new OneEuroFilter(minCutoff, beta, dCutoff);
        _y = new OneEuroFilter(minCutoff, beta, dCutoff);
    }

    public void SetParams(float minCutoff, float beta)
    {
        _x.MinCutoff = minCutoff; _x.Beta = beta;
        _y.MinCutoff = minCutoff; _y.Beta = beta;
    }

    public Vector2 Filter(Vector2 v, float dt)
    {
        return new Vector2(_x.Filter(v.x, dt), _y.Filter(v.y, dt));
    }

    /// <summary>
    /// 平滑化した上で、速度を使って predictSeconds 秒だけ位置を前方外挿する。
    /// 30fps固定などで生じる更新遅延を体感上相殺するために使う。
    /// maxPredictDelta: 予測による1軸あたりの最大移動量（手の再検出で速度が跳ねた際の暴走防止）。
    /// </summary>
    public Vector2 FilterPredicted(Vector2 v, float dt, float predictSeconds, float maxPredictDelta)
    {
        Vector2 filtered = new Vector2(_x.Filter(v.x, dt), _y.Filter(v.y, dt));
        if (predictSeconds <= 0f) return filtered;

        float dxp = Mathf.Clamp(_x.Velocity * predictSeconds, -maxPredictDelta, maxPredictDelta);
        float dyp = Mathf.Clamp(_y.Velocity * predictSeconds, -maxPredictDelta, maxPredictDelta);
        return new Vector2(filtered.x + dxp, filtered.y + dyp);
    }

    public void Reset()
    {
        _x.Reset();
        _y.Reset();
    }
}
