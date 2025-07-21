using UnityEngine;
using DG.Tweening;

public class TargetObject : MonoBehaviour
{
    public float moveRange = 40f;  // ← 動く範囲を大幅に拡張
    public float moveDurationMin = 0.5f;
    public float moveDurationMax = 1.5f;
    public float waitTimeMin = 0.3f;
    public float waitTimeMax = 1.2f;
    public Ease moveEase = Ease.InOutSine;

    private Vector3 originPosition;
    private Tween moveTween;

    void Start()
    {
        originPosition = transform.position;
        ScheduleNextMove();
    }

    void ScheduleNextMove()
    {
        Vector3 offset = new Vector3(
            Random.Range(-moveRange, moveRange),
            Random.Range(-moveRange, moveRange),
            0f
        );

        Vector3 targetPosition = originPosition + offset;
        float duration = Random.Range(moveDurationMin, moveDurationMax);
        float wait = Random.Range(waitTimeMin, waitTimeMax);

        moveTween = transform.DOMove(targetPosition, duration)
            .SetEase(moveEase)
            .OnComplete(() => Invoke(nameof(ScheduleNextMove), wait));
    }

    public void DestroyWithEffect()
    {
        moveTween?.Kill();
        Destroy(gameObject);
    }
}
