using UnityEngine;
using extOSC;

public class OSCHandReceiver : MonoBehaviour
{
    public OSCReceiver receiver;
    public string leftAddress = "/hand/left_palm";
    public string rightAddress = "/hand/right_palm";
    public float scale = 500f;

    void Start()
    {
        receiver.Bind(leftAddress, OnLeftPalmReceived);
        receiver.Bind(rightAddress, OnRightPalmReceived);
    }

    void OnLeftPalmReceived(OSCMessage message)
    {
        if (message.Values.Count >= 3)
        {
            float x = message.Values[0].FloatValue;
            float y = message.Values[1].FloatValue;
            bool grab = message.Values[2].FloatValue > 0.5f;
            PalmDataManager.LeftPalm = PalmDataManager.ConvertNormalizedToWorld(x, y, scale);
            PalmDataManager.LeftGrabbing = grab;
        }
    }

    void OnRightPalmReceived(OSCMessage message)
    {
        if (message.Values.Count >= 3)
        {
            float x = message.Values[0].FloatValue;
            float y = message.Values[1].FloatValue;
            bool grab = message.Values[2].FloatValue > 0.5f;
            PalmDataManager.RightPalm = PalmDataManager.ConvertNormalizedToWorld(x, y, scale);
            PalmDataManager.RightGrabbing = grab;
        }
    }
}
