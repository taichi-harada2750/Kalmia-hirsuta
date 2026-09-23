using UnityEngine;

namespace KIS.Core
{
    ///<summary>
    /// 手の座標・状態を統一的に管理(PalmDataManagerの後継)
    ///</summary>
    public class PointerManager
    {
        public Vector3 LeftPos => leftPos;
        public Vector3 RightPos => rightPos;
        public Vector3 MousePos => mousePos; // 追加
        public bool LeftGrab => leftGrab && LeftTracked;
        public bool RightGrab => rightGrab && RightTracked;
        public bool MouseGrab => mouseGrab && MouseTracked;   // 追加
        public bool LeftTracked => IsTracked(leftLastUpdateTime, leftReportedTracked);
        public bool RightTracked => IsTracked(rightLastUpdateTime, rightReportedTracked);
        public bool MouseTracked => IsTracked(mouseLastUpdateTime, mouseReportedTracked);

        private const float smooth = 0.3f;
        private const float trackingTimeout = 0.25f;

        private float leftLastUpdateTime = -1f;
        private float rightLastUpdateTime = -1f;
        private float mouseLastUpdateTime = -1f;
        private Vector3 leftPos;
        private Vector3 rightPos;
        private Vector3 mousePos;
        private bool leftGrab;
        private bool rightGrab;
        private bool mouseGrab;
        private bool leftReportedTracked;
        private bool rightReportedTracked;
        private bool mouseReportedTracked;

        public void UpdateLeft(Vector3 newpos, bool grab, bool tracked = true)
        {
            UpdatePosition(ref leftLastUpdateTime, newpos, tracked,
                ref leftPos, ref leftReportedTracked);
            leftGrab = grab;
        }

        public void UpdateRight(Vector3 newpos, bool grab, bool tracked = true)
        {
            UpdatePosition(ref rightLastUpdateTime, newpos, tracked,
                ref rightPos, ref rightReportedTracked);
            rightGrab = grab;
        }

        public void UpdateMouse(Vector3 newpos, bool grab, bool tracked = true)
        {
            UpdatePosition(ref mouseLastUpdateTime, newpos, tracked,
                ref mousePos, ref mouseReportedTracked);
            mouseGrab = grab;
        }

        private static void UpdatePosition(ref float lastUpdateTime, Vector3 newPosition,
            bool tracked, ref Vector3 position, ref bool reportedTracked)
        {
            if (tracked)
            {
                position = Vector3.Lerp(position, newPosition, smooth);
                lastUpdateTime = Time.time;
            }

            reportedTracked = tracked;
        }

        private static bool IsTracked(float lastUpdateTime, bool reportedTracked)
        {
            return reportedTracked && lastUpdateTime >= 0f &&
                Time.time - lastUpdateTime <= trackingTimeout;
        }

    }
}
