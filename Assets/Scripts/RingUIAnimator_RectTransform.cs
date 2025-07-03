using UnityEngine;
using DG.Tweening;

public class RingUIAnimator_RectTransform : MonoBehaviour
{
    public RectTransform[] iconRects;
    public float radius = 200f; // UI用なので単位はピクセル
    public float animationDuration = 0.5f;

    public void PlaySummonAnimation()
    {
        int count = iconRects.Length;
        for (int i = 0; i < count; i++)
        {
            RectTransform icon = iconRects[i];
            float angle = 360f / count * i;
            Vector2 offset = new Vector2(
                Mathf.Cos(angle * Mathf.Deg2Rad),
                Mathf.Sin(angle * Mathf.Deg2Rad)
            ) * radius;

            icon.anchoredPosition = Vector2.zero;
            icon.localScale = Vector3.zero;

            icon.DOAnchorPos(offset, animationDuration).SetEase(Ease.OutBack);
            icon.DOScale(Vector3.one, animationDuration).SetEase(Ease.OutBack);
        }
    }
}
