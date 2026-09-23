
using UnityEngine;
using UnityEngine.UI;

public class HandHoldIndicator : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private Image progressFill;
    [SerializeField] private GameObject progressTrack;
    [SerializeField] private GameObject holdMarker;

    [Header("Timing")]
    [SerializeField] private float showDelay = 0.2f;
    [SerializeField] private float completeDuration = 0.2f;

    private enum State
    {
        Hidden,
        Charging,
        Completed,
        Holding
    }

    private State state = State.Hidden;
    private float completedAt;

    private void Awake()
    {
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
        ResetIndicator();
    }

    public void SetProgress(float progress, float elapsed)
    {
        if (state == State.Completed ||
            state == State.Holding)
            return;

        if (elapsed < showDelay)
        {
            ResetIndicator();
            return;
        }

        state = State.Charging;
        canvasGroup.alpha = 1f;

        progressTrack.SetActive(true);
        holdMarker.SetActive(false);

        progressFill.fillAmount =
            Mathf.Clamp01(progress);
    }

    public void OnHold()
    {
        state = State.Completed;
        completedAt = Time.unscaledTime;

        canvasGroup.alpha = 1f;
        progressTrack.SetActive(true);
        holdMarker.SetActive(false);
        progressFill.fillAmount = 1f;
    }

    public void OnRelease()
    {
        ResetIndicator();
    }

    private void Update()
    {
        if (state == State.Completed &&
            Time.unscaledTime - completedAt >=
            completeDuration)
        {
            state = State.Holding;

            progressTrack.SetActive(false);
            holdMarker.SetActive(true);
        }
    }

    private void ResetIndicator()
    {
        state = State.Hidden;
        canvasGroup.alpha = 0f;

        progressTrack.SetActive(false);
        holdMarker.SetActive(false);
        progressFill.fillAmount = 0f;
    }
}