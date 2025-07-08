using UnityEngine;
using DG.Tweening;

public class UIHoverEffectPulsing : MonoBehaviour
{
    public Vector3 initialScale = Vector3.one;　// ← Inspectorで事前指定
    private Vector3 originalScale;
    private Tween pulseTween;
    private bool isHovering = false;

    public float pulseScale = 1.2f;  // ← 常識的な拡大倍率に修正
    public float pulseDuration = 0.4f;

    void OnEnable()
    {
        originalScale = initialScale;
        transform.localScale = originalScale;
    }

    public void SetHover(bool hovering)
    {
        if (isHovering == hovering) return;

        isHovering = hovering;

        if (pulseTween != null) pulseTween.Kill();

        if (hovering)
        {
            pulseTween = transform.DOScale(originalScale * pulseScale, pulseDuration)
                .SetEase(Ease.InOutSine)
                .SetLoops(-1, LoopType.Yoyo);
        }
        else
        {
            pulseTween = transform.DOScale(originalScale, 0.2f).SetEase(Ease.OutBack);
        }
    }

    public void PlayClickEffect()
    {
        if (pulseTween != null) pulseTween.Kill();

        transform.DOScale(originalScale * 1.3f, 0.1f)
            .SetEase(Ease.OutQuad)
            .OnComplete(() =>
            {
                transform.DOScale(originalScale, 0.15f).SetEase(Ease.OutBack);
            });
    }
}
