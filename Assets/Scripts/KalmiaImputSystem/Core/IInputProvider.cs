using System;
using UnityEngine;

namespace KIS.Core
{
    public enum KISHand
    {
        Left,
        Right,
        Mouse
    }

    /// <summary>
    /// 各入力デバイス（Mediapipe, Kinect, UnityInputなど）の共通インタフェース。
    /// </summary>
    public interface IInputProvider
    {
        //public明示 + Action引数明確化
        public event Action<HandData> OnLeftHandUpdated;
        public event Action<HandData> OnRightHandUpdated;
        public event Action<HandData> OnMouseUpdated;

        public void Initialize();
        public void UpdateProvider();
    }

    [Serializable]
    public struct HandData
    {
        public Vector3 position;
        public Vector3 velocity;
        public bool isGrabbing;
        public float timestamp;
        public KISHand hand;
        public bool isTracked;

        public HandData(Vector3 pos, Vector3 vel, bool grab, float time)
            : this(pos, vel, grab, time, KISHand.Mouse, true)
        {
        }

        public HandData(Vector3 pos, Vector3 vel, bool grab, float time, KISHand hand, bool tracked)
        {
            position = pos;
            velocity = vel;
            isGrabbing = grab;
            timestamp = time;
            this.hand = hand;
            isTracked = tracked;
        }
    }
}
