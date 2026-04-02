using UnityEngine;
using KIS.Core;

namespace KIS.Core
{
    /// <summary>
    /// KIS全体を統括し、Providerからデータを受けてIntent処理を流す。
    /// </summary>
    public class KISManager : MonoBehaviour
    {
        [Tooltip("入力Providerのリスト（Mediapipe, UnityMouseなど複数登録可）")]
        public MonoBehaviour[] inputProviderComponents;
        private System.Collections.Generic.List<IInputProvider> inputProviders = new System.Collections.Generic.List<IInputProvider>();

        private PointerManager pointerManager = new PointerManager();
        private GestureRecognizer rightGestureRecognizer = new GestureRecognizer();
        private GestureRecognizer leftGestureRecognizer = new GestureRecognizer();
        private GestureRecognizer mouseGestureRecognizer = new GestureRecognizer();
        
        // OutputAdapter等からアクセスできるようにプロパティ化
        public IntentInterpreter IntentInterpreter { get; private set; } = new IntentInterpreter();

        [Tooltip("Hover対象（UIオブジェクトなど）")]
        public Transform hoverTarget;

        void Awake()
        {
            if (inputProviderComponents != null)
            {
                foreach (var comp in inputProviderComponents)
                {
                    if (comp is IInputProvider provider)
                    {
                        provider.OnLeftHandUpdated += OnLeftHandUpdated;
                        provider.OnRightHandUpdated += OnRightHandUpdated;
                        provider.OnMouseUpdated += OnMouseUpdated;
                        provider.Initialize();
                        inputProviders.Add(provider);
                    }
                    else if (comp != null)
                    {
                        Debug.LogWarning($"[KIS] {comp.name} は IInputProvider を実装していません。");
                    }
                }
            }
        }

        void Update()
        {
            foreach (var provider in inputProviders)
            {
                provider.UpdateProvider();
            }

            // Right Hand
            HandData right = new HandData(pointerManager.RightPos, Vector3.zero, pointerManager.RightGrab, Time.time);
            IntentType rightIntent = rightGestureRecognizer.Recognize(right, hoverTarget ? hoverTarget.position : Vector3.zero);
            IntentInterpreter.Process(rightIntent, right);

            // Left Hand
            HandData left = new HandData(pointerManager.LeftPos, Vector3.zero, pointerManager.LeftGrab, Time.time);
            IntentType leftIntent = leftGestureRecognizer.Recognize(left, hoverTarget ? hoverTarget.position : Vector3.zero);
            IntentInterpreter.Process(leftIntent, left);

            // Mouse (Third Hand)
            HandData mouse = new HandData(pointerManager.MousePos, Vector3.zero, pointerManager.MouseGrab, Time.time);
            IntentType mouseIntent = mouseGestureRecognizer.Recognize(mouse, hoverTarget ? hoverTarget.position : Vector3.zero);
            IntentInterpreter.Process(mouseIntent, mouse);
        }

        private void OnLeftHandUpdated(HandData data)
        {
            pointerManager.UpdateLeft(data.position, data.isGrabbing);
        }

        private void OnRightHandUpdated(HandData data)
        {
            pointerManager.UpdateRight(data.position, data.isGrabbing);
        }

        private void OnMouseUpdated(HandData data)
        {
            pointerManager.UpdateMouse(data.position, data.isGrabbing);
        }
    }
}
