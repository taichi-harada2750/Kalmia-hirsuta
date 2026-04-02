using System;
using UnityEngine;

namespace KIS.Core
{
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

        public HandData(Vector3 pos, Vector3 vel, bool grab, float time)
        {
            position = pos;
            velocity = vel;
            isGrabbing = grab;
            timestamp = time;
        }
    }
}
