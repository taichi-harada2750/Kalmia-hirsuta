using UnityEngine;
using System;
using KIS.Core;

namespace KIS.Providers
{
    /// <summary>
    /// 既存の MediaPipeOSCReceiver / PalmDataManager からデータを受け取り、
    /// KISが要求する IInputProvider の形式（HandData）に変換して流すラッパークラス。
    /// </summary>
    public class MediapipeProvider : MonoBehaviour, IInputProvider
    {
        public event Action<HandData> OnLeftHandUpdated;
        public event Action<HandData> OnRightHandUpdated;

        private Vector3 lastLeftPos;
        private Vector3 lastRightPos;

        public void Initialize()
        {
            Debug.Log("[KIS] MediapipeProvider Initialized. Subscribing to PalmDataManager.");
        }

        public void UpdateProvider()
        {
            // PalmDataManager からデータを取得（MediaPipeOSCReceiverがバックグラウンドで更新している）
            Vector3 currentLeftPos = PalmDataManager.LeftPalm;
            bool isLeftGrabbing = PalmDataManager.LeftGrabbing;
            // 速度計算 (m/s)
            Vector3 leftVel = (Time.deltaTime > 0) ? (currentLeftPos - lastLeftPos) / Time.deltaTime : Vector3.zero;
            
            HandData leftData = new HandData(currentLeftPos, leftVel, isLeftGrabbing, Time.time);
            OnLeftHandUpdated?.Invoke(leftData);
            lastLeftPos = currentLeftPos;

            Vector3 currentRightPos = PalmDataManager.RightPalm;
            bool isRightGrabbing = PalmDataManager.RightGrabbing;
            // 速度計算 (m/s)
            Vector3 rightVel = (Time.deltaTime > 0) ? (currentRightPos - lastRightPos) / Time.deltaTime : Vector3.zero;
            
            HandData rightData = new HandData(currentRightPos, rightVel, isRightGrabbing, Time.time);
            OnRightHandUpdated?.Invoke(rightData);
            lastRightPos = currentRightPos;
        }
    }
}
