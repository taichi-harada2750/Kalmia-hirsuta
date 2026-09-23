using UnityEngine;
using System;
using KIS.Core;
using UnityEngine.EventSystems;

namespace KIS.Output
{
    /// <summary>
    /// KISCore から出力された Intent（ジェスチャー意図）を受け取り、
    /// Unityの既存UI層へイベントとして伝達するアダプタークラス。
    /// </summary>
    public class UnityUIOutputAdapter : MonoBehaviour
    {
        [Tooltip("統括する KISManager")]
        public KISManager kisManager;

        // UIスクリプトが購読しやすいように各アクションごとのイベントを公開
        public event Action<Vector3> OnGrabDetected;
        public event Action<Vector3> OnReleaseDetected;
        public event Action<Vector3> OnHoverDetected;
        public event Action<Vector3> OnSwipeDetected;
        public event Action<Vector3> OnHoldDetected;

        // 既存の位置のみのイベントを維持しつつ、KISHandを識別できるイベントを追加。
        public event Action<HandData> OnGrabDetectedWithHand;
        public event Action<HandData> OnReleaseDetectedWithHand;
        public event Action<HandData> OnHoldDetectedWithHand;

        private void OnEnable()
        {
            if (kisManager != null)
            {
                // KISManagerが持つ IntentInterpreter に接続し、Intent発火を監視
                kisManager.IntentInterpreter.OnIntentDetected += HandleIntent;
                Debug.Log("[KIS] UnityUIOutputAdapter Subscribed to IntentInterpreter");
            }
            else
            {
                Debug.LogWarning("[KIS] UnityUIOutputAdapter: KISManager is not assigned!");
            }
        }

        private void OnDisable()
        {
            if (kisManager != null && kisManager.IntentInterpreter != null)
            {
                kisManager.IntentInterpreter.OnIntentDetected -= HandleIntent;
            }
        }

        private void HandleIntent(IntentType intent, HandData handData)
        {
            // Intentの種類に応じて専用のイベントに振り分けて発火
            switch (intent)
            {
                case IntentType.Grab:
                    OnGrabDetected?.Invoke(handData.position);
                    OnGrabDetectedWithHand?.Invoke(handData);
                    break;

                case IntentType.Release:
                    OnReleaseDetected?.Invoke(handData.position);
                    OnReleaseDetectedWithHand?.Invoke(handData);
                    break;

                case IntentType.Hover:
                    OnHoverDetected?.Invoke(handData.position);
                    break;

                case IntentType.Hold:
                    OnHoldDetected?.Invoke(handData.position);
                    OnHoldDetectedWithHand?.Invoke(handData);
                    break;

                case IntentType.Swipe:
                    OnSwipeDetected?.Invoke(handData.velocity);
                    break;
            }
        }

        void OnDestroy()
        {
            if (kisManager != null && kisManager.IntentInterpreter != null)
            {
                kisManager.IntentInterpreter.OnIntentDetected -= HandleIntent;
            }
        }
    }
}
