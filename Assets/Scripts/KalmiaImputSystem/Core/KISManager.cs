using UnityEngine;
using KIS.Core;

namespace KIS.Core
{
    /// <summary>
    /// KIS全体を統括し、Providerからデータを受けてIntent処理を流す。
    /// </summary>
    public class KISManager : MonoBehaviour
    {
        [Tooltip("入力Provider（例：MediapipeProvider, KinectProviderなど）")]
        public MonoBehaviour inputProviderComponent; // IInputProviderを継承していることを前提
        private IInputProvider inputProvider;

        private PointerManager pointerManager = new PointerManager();
        private GestureRecognizer gestureRecognizer = new GestureRecognizer();
        private IntentInterpreter intentInterpreter = new IntentInterpreter();

        [Tooltip("Hover対象（UIオブジェクトなど）")]
        public Transform hoverTarget;

        void Awake()
        {
            inputProvider = inputProviderComponent as IInputProvider;
            if (inputProvider == null)
            {
                Debug.LogError("[KIS] 入力Providerが正しく設定されていません。");
                return;
            }

            inputProvider.OnLeftHandUpdated += OnLeftHandUpdated;
            inputProvider.OnRightHandUpdated += OnRightHandUpdated;
            inputProvider.Initialize();
        }

        void Update()
        {
            inputProvider?.UpdateProvider();

            HandData right = new HandData(pointerManager.RightPos, Vector3.zero, pointerManager.RightGrab, Time.time);
            IntentType intent = gestureRecognizer.Recognize(right, hoverTarget ? hoverTarget.position : Vector3.zero);
            intentInterpreter.Process(intent, right);
        }

        private void OnLeftHandUpdated(HandData data)
        {
            pointerManager.UpdateLeft(data.position, data.isGrabbing);
        }

        private void OnRightHandUpdated(HandData data)
        {
            pointerManager.UpdateRight(data.position, data.isGrabbing);
        }
    }
}
