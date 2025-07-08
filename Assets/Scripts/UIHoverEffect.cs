using UnityEngine;
using DG.Tweening;

public class UIHoverEffect : MonoBehaviour
{
    private Vector3 originalScale;
    private bool isHovering = false;
    private Tween scaleTween;

    public Color normalColor = Color.white;
    public Color highlightColor = new Color(0.7f, 0.9f, 1f, 1f);
    public float hoverScale = 1.1f;
    public float animDuration = 0.2f;

    private UnityEngine.UI.Image image;

    void Start()
    {
        originalScale = transform.localScale;
        image = GetComponent<UnityEngine.UI.Image>();
        if (image != null)
            image.color = normalColor;
    }

    public void SetHover(bool hovering)
    {
        if (isHovering == hovering) return;

        isHovering = hovering;

        if (scaleTween != null) scaleTween.Kill();

        if (hovering)
        {
            scaleTween = transform.DOScale(originalScale * hoverScale, animDuration).SetEase(Ease.OutQuad);
            if (image != null) image.DOColor(highlightColor, animDuration);
        }
        else
        {
            scaleTween = transform.DOScale(originalScale, animDuration).SetEase(Ease.OutQuad);
            if (image != null) image.DOColor(normalColor, animDuration);
        }
    }

    public void PlayClickEffect()
    {
        // 発光＆拡大＆消滅
        Sequence seq = DOTween.Sequence();
        seq.Append(transform.DOScale(originalScale * 1.4f, 0.15f));
        seq.Join(image.DOFade(0f, 0.15f));
        seq.OnComplete(() => gameObject.SetActive(false));
    }
}
