using UnityEngine;
using UnityEngine.Events;

public class KeyEventTrigger : MonoBehaviour
{
    [System.Serializable]
    public class KeyEvent
    {
        public KeyCode key;
        public UnityEvent onKeyPress;
    }

    public KeyEvent[] keyEvents;

    void Update()
    {
        foreach (var ke in keyEvents)
        {
            if (Input.GetKeyDown(ke.key))
            {
                ke.onKeyPress?.Invoke();
            }
        }
    }
}
