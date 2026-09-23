using UnityEngine;
using KIS.Core;

namespace KIS.Core
{
    public struct KISHandState
    {
        public KISHand hand;
        public Vector3 position;
        public bool isTracked;
        public bool isGrabbing;
        public bool isHolding;
        public float grabElapsed;
        public float holdTime;
        public float holdProgress;

        public KISHandState(KISHand hand, Vector3 position, bool isTracked,
            bool isGrabbing, bool isHolding, float grabElapsed, float holdTime,
            float holdProgress)
        {
            this.hand = hand;
            this.position = position;
            this.isTracked = isTracked;
            this.isGrabbing = isGrabbing;
            this.isHolding = isHolding;
            this.grabElapsed = grabElapsed;
            this.holdTime = holdTime;
            this.holdProgress = holdProgress;
        }
    }

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

        public event System.Action<KISHandState> OnHandStateUpdated;
        
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
            Vector3 hoverPosition = hoverTarget ? hoverTarget.position : Vector3.zero;

            HandData right = new HandData(pointerManager.RightPos, Vector3.zero,
                pointerManager.RightGrab, Time.time, KISHand.Right, pointerManager.RightTracked);
            IntentType rightIntent = rightGestureRecognizer.Recognize(right, hoverPosition);
            IntentInterpreter.Process(rightIntent, right);
            PublishHandState(KISHand.Right, right, rightGestureRecognizer);

            // Left Hand
            HandData left = new HandData(pointerManager.LeftPos, Vector3.zero,
                pointerManager.LeftGrab, Time.time, KISHand.Left, pointerManager.LeftTracked);
            IntentType leftIntent = leftGestureRecognizer.Recognize(left, hoverPosition);
            IntentInterpreter.Process(leftIntent, left);
            PublishHandState(KISHand.Left, left, leftGestureRecognizer);

            // Mouse (Third Hand)
            HandData mouse = new HandData(pointerManager.MousePos, Vector3.zero,
                pointerManager.MouseGrab, Time.time, KISHand.Mouse, pointerManager.MouseTracked);
            IntentType mouseIntent = mouseGestureRecognizer.Recognize(mouse, hoverPosition);
            IntentInterpreter.Process(mouseIntent, mouse);
            PublishHandState(KISHand.Mouse, mouse, mouseGestureRecognizer);
        }

        public bool TryGetHandState(KISHand hand, out KISHandState state)
        {
            switch (hand)
            {
                case KISHand.Right:
                    state = CreateHandState(hand, pointerManager.RightPos,
                        pointerManager.RightTracked, pointerManager.RightGrab, rightGestureRecognizer);
                    return true;
                case KISHand.Left:
                    state = CreateHandState(hand, pointerManager.LeftPos,
                        pointerManager.LeftTracked, pointerManager.LeftGrab, leftGestureRecognizer);
                    return true;
                case KISHand.Mouse:
                    state = CreateHandState(hand, pointerManager.MousePos,
                        pointerManager.MouseTracked, pointerManager.MouseGrab, mouseGestureRecognizer);
                    return true;
                default:
                    state = default(KISHandState);
                    return false;
            }
        }

        private void PublishHandState(KISHand hand, HandData data, GestureRecognizer recognizer)
        {
            OnHandStateUpdated?.Invoke(CreateHandState(hand, data.position,
                data.isTracked, data.isGrabbing, recognizer));
        }

        private static KISHandState CreateHandState(KISHand hand, Vector3 position,
            bool isTracked, bool isGrabbing, GestureRecognizer recognizer)
        {
            return new KISHandState(hand, position, isTracked, isGrabbing,
                recognizer.IsHolding, recognizer.GrabElapsed, recognizer.HoldTime,
                recognizer.HoldProgress);
        }

        private void OnLeftHandUpdated(HandData data)
        {
            pointerManager.UpdateLeft(data.position, data.isGrabbing, data.isTracked);
        }

        private void OnRightHandUpdated(HandData data)
        {
            pointerManager.UpdateRight(data.position, data.isGrabbing, data.isTracked);
        }

        private void OnMouseUpdated(HandData data)
        {
            pointerManager.UpdateMouse(data.position, data.isGrabbing, data.isTracked);
        }
    }
}
