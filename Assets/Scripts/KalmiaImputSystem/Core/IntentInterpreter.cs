using System
using UnityEngine;

namespace KIS.Core
{
    ///<summary>
    /// GestureRecognizerで検出されたIntentを発火・中継する。
    /// </summary>
    public class IntentInterpreter
    {
        public event Action<IntentType, HandData> OnIntentDetected;

        public void Process(IntentType intent, HandData handData)
        {
            if (intent != IntentType.None)
            {
                Debug.Log($"[KIS] Intent Detected: {intent}");
                OnIntentDetected?.Invoke(intent, handData);
            }
        }


    }
}