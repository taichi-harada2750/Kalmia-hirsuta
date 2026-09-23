using UnityEngine;

namespace KIS.Core
{
    public enum IntentType { None, Grab, Release, Hover, Swipe, Hold }

    public class GestureRecognizer
    {
        private const float swipeThreshold = 0.25f;
        private const float holdTime = 1.0f;
        private const float hoverDistance = 0.02f;

        private bool wasGrabbing = false;
        private float grabStartTime = 0f;
        private bool isFirstFrame = true;

        private bool holdTriggered = false;

        public IntentType Recognize(HandData current, Vector3 targetPosition)
        {
            if (isFirstFrame)
            {
                wasGrabbing = current.isGrabbing;
                if (current.isGrabbing)
                {
                    grabStartTime = Time.time;
                }
                isFirstFrame = false;
                // 初回フレームで既に握っていた場合は、今回発火させずにホールド状態として扱う
            }

            // Grab / Release
            if (current.isGrabbing && !wasGrabbing)
            {
                grabStartTime = Time.time;
                holdTriggered = false;
                wasGrabbing = true;

                return IntentType.Grab;
            }
            else if (!current.isGrabbing && wasGrabbing)
            {
                wasGrabbing = false;
                holdTriggered = false;
                grabStartTime = 0f;
                return IntentType.Release;
            }

            // Hold
            if (current.isGrabbing && !holdTriggered && Time.time - grabStartTime >= holdTime)
            {
                holdTriggered = true;
                return IntentType.Hold;
            }

            // Swipe
            if (current.isGrabbing && current.velocity.magnitude > swipeThreshold)
                return IntentType.Swipe;

            // Hover
            if (Vector3.Distance(current.position, targetPosition) < hoverDistance && !current.isGrabbing)
                return IntentType.Hover;

            return IntentType.None;
        }
    }
}
