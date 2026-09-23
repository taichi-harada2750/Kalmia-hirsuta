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
        public event Action<HandData> OnMouseUpdated; // インターフェース実装のため追加

        private Vector3 lastLeftPos;
        private Vector3 lastRightPos;

        public void Initialize()
        {
            Debug.Log("[KIS] MediapipeProviderが初期化されました。PalmDataManagerと接続しています。");
        }

        public void UpdateProvider()
        {
            // PalmDataManager からデータを取得（MediaPipeOSCReceiverがバックグラウンドで更新している）
            Vector3 currentLeftPos = PalmDataManager.LeftPalm;
            bool isLeftGrabbing = PalmDataManager.LeftGrabbing;
            // 速度計算 (m/s)
            Vector3 leftVel = (Time.deltaTime > 0) ? (currentLeftPos - lastLeftPos) / Time.deltaTime : Vector3.zero;
            
            HandData leftData = new HandData(currentLeftPos, leftVel, isLeftGrabbing,
                Time.time, KISHand.Left, PalmDataManager.LeftTracked);
            OnLeftHandUpdated?.Invoke(leftData);
            lastLeftPos = currentLeftPos;

            Vector3 currentRightPos = PalmDataManager.RightPalm;
            bool isRightGrabbing = PalmDataManager.RightGrabbing;
            // 速度計算 (m/s)
            Vector3 rightVel = (Time.deltaTime > 0) ? (currentRightPos - lastRightPos) / Time.deltaTime : Vector3.zero;
            
            HandData rightData = new HandData(currentRightPos, rightVel, isRightGrabbing,
                Time.time, KISHand.Right, PalmDataManager.RightTracked);
            OnRightHandUpdated?.Invoke(rightData);
            lastRightPos = currentRightPos;
        }
    }
}
