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

        public IntentType Recognize(HandData current, Vector3 targetPosition)
        {
            if (isFirstFrame)
            {
                wasGrabbing = current.isGrabbing;
                isFirstFrame = false;
                // 初回フレームで既に握っていた場合は、今回発火させずにホールド状態として扱う
            }
            // Hover
            if (Vector3.Distance(current.position, targetPosition) < hoverDistance && !current.isGrabbing)
                return IntentType.Hover;

            // Grab / Release
            if (current.isGrabbing && !wasGrabbing)
            {
                grabStartTime = Time.time;
                wasGrabbing = true;
                return IntentType.Grab;
            }
            else if (!current.isGrabbing && wasGrabbing)
            {
                wasGrabbing = false;
                return IntentType.Release;
            }

            // Hold
            if (current.isGrabbing && Time.time - grabStartTime > holdTime)
                return IntentType.Hold;

            // Swipe
            if (current.isGrabbing && current.velocity.magnitude > swipeThreshold)
                return IntentType.Swipe;

            return IntentType.None;
        }
    }
}
