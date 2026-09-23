using UnityEngine;
using KIS.Core;
using KIS.Output;

public class HandHoldIndicatorBridge : MonoBehaviour
{
    [Header("KIS")]
    [SerializeField] private KISManager kisManager;
    [SerializeField] private UnityUIOutputAdapter outputAdapter;
    [SerializeField] private KISHand hand = KISHand.Right;

    [Header("Indicator")]
    [SerializeField] private HandHoldIndicator indicator;

    [Header("Follow")]
    [Tooltip("既存の手ポインタ。未指定の場合はKISのPointerManager座標を使用します。")]
    [SerializeField] private Transform handPointer;
    [Tooltip("HandPointerから下方向へずらす表示オフセット。UIではCanvas親のローカル座標です。")]
    [SerializeField] private Vector3 positionOffset = new Vector3(0f, -40f, 0f);
    [SerializeField] private RectTransform indicatorRect;
    [SerializeField] private Canvas targetCanvas;

    private bool subscribed;
    private bool grabActive;
    private bool holdNotified;
    private bool hasState;
    private Vector3 lastKisPosition;

    private void OnEnable()
    {
        if (subscribed)
            return;

        ResolveReferences();

        if (kisManager != null)
        {
            kisManager.OnHandStateUpdated += HandleHandStateUpdated;
        }

        if (outputAdapter != null)
        {
            outputAdapter.OnGrabDetectedWithHand += HandleGrab;
            outputAdapter.OnHoldDetectedWithHand += HandleHold;
            outputAdapter.OnReleaseDetectedWithHand += HandleRelease;
        }

        subscribed = true;
        ResetIndicator();
    }

    private void OnDisable()
    {
        if (!subscribed)
            return;

        if (kisManager != null)
        {
            kisManager.OnHandStateUpdated -= HandleHandStateUpdated;
        }

        if (outputAdapter != null)
        {
            outputAdapter.OnGrabDetectedWithHand -= HandleGrab;
            outputAdapter.OnHoldDetectedWithHand -= HandleHold;
            outputAdapter.OnReleaseDetectedWithHand -= HandleRelease;
        }

        subscribed = false;
        ResetIndicator();
    }

    private void LateUpdate()
    {
        // KISの状態を表示するだけで、ここでは経過時間を計測しない。
        // HandPointerが非表示でもTransformの座標は追従対象として利用する。
        if (hasState && grabActive)
        {
            ApplyPosition(GetFollowPosition());
        }
    }

    private void HandleGrab(HandData data)
    {
        if (data.hand != hand)
            return;

        grabActive = true;
        holdNotified = false;
        hasState = true;
        lastKisPosition = data.position;
        ApplyPosition(GetFollowPosition());
    }

    private void HandleHold(HandData data)
    {
        if (data.hand != hand || holdNotified)
            return;

        grabActive = true;
        holdNotified = true;
        hasState = true;
        lastKisPosition = data.position;
        ApplyPosition(GetFollowPosition());
        indicator?.OnHold();
    }

    private void HandleRelease(HandData data)
    {
        if (data.hand != hand)
            return;

        ResetIndicator();
    }

    private void HandleHandStateUpdated(KISHandState state)
    {
        if (state.hand != hand)
            return;

        hasState = true;
        lastKisPosition = state.position;
        ApplyPosition(GetFollowPosition());

        if (!state.isTracked || !state.isGrabbing)
        {
            // Releaseイベントが無効化中に発生した場合や、追跡が途切れた場合も
            // staleな表示を残さない。
            if (grabActive || holdNotified)
            {
                ResetIndicator();
            }
            return;
        }

        if (!grabActive)
        {
            // Bridgeの再有効化がGrabイベントの後でも、KISの現在状態から復帰する。
            grabActive = true;
            holdNotified = false;
        }

        if (state.isHolding)
        {
            if (!holdNotified)
            {
                holdNotified = true;
                indicator?.OnHold();
            }
        }
        else if (!holdNotified)
        {
            indicator?.SetProgress(state.holdProgress, state.grabElapsed);
        }
    }

    private void ResolveReferences()
    {
        if (indicator == null)
            indicator = GetComponent<HandHoldIndicator>();

        if (outputAdapter == null)
            outputAdapter = FindObjectOfType<UnityUIOutputAdapter>();

        if (kisManager == null && outputAdapter != null)
            kisManager = outputAdapter.kisManager;

        if (kisManager == null)
            kisManager = FindObjectOfType<KISManager>();

        if (indicatorRect == null && indicator != null)
            indicatorRect = indicator.GetComponent<RectTransform>();

        if (targetCanvas == null && indicatorRect != null)
            targetCanvas = indicatorRect.GetComponentInParent<Canvas>();
    }

    private void ResetIndicator()
    {
        grabActive = false;
        holdNotified = false;
        hasState = false;
        indicator?.OnRelease();
    }

    private Vector3 GetFollowPosition()
    {
        return handPointer != null ? handPointer.position : lastKisPosition;
    }

    private void ApplyPosition(Vector3 worldPosition)
    {
        if (indicatorRect == null)
        {
            transform.position = worldPosition + positionOffset;
            return;
        }

        if (targetCanvas == null)
        {
            indicatorRect.position = worldPosition + positionOffset;
            return;
        }

        RectTransform parent = indicatorRect.parent as RectTransform;
        if (parent == null)
        {
            indicatorRect.position = worldPosition + positionOffset;
            return;
        }

        Camera eventCamera = targetCanvas.renderMode == RenderMode.ScreenSpaceOverlay
            ? null
            : targetCanvas.worldCamera != null ? targetCanvas.worldCamera : Camera.main;
        Vector2 screenPosition = ToScreenPosition(worldPosition, eventCamera);

        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            parent, screenPosition, eventCamera, out Vector2 localPosition))
        {
            indicatorRect.anchoredPosition = localPosition +
                new Vector2(positionOffset.x, positionOffset.y);
        }
    }

    private static Vector2 ToScreenPosition(Vector3 worldPosition, Camera eventCamera)
    {
        // Screen Space OverlayではKISのUI座標が既に画面座標として扱える。
        if (eventCamera == null)
            return new Vector2(worldPosition.x, worldPosition.y);

        return eventCamera.WorldToScreenPoint(worldPosition);
    }
}
