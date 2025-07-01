using UnityEngine;
using extOSC;

public class OSCHandReceiver : MonoBehaviour
{
    public OSCReceiver receiver;
    public string leftAddress = "/hand/left_palm";
    public string rightAddress = "/hand/right_palm";
    public float scale = 500f;
    public RectTransform canvasRect; // CanvasのRectTransformをインスペクターで指定

    void Start()
    {
        receiver.Bind(leftAddress, OnLeftPalmReceived);
        receiver.Bind(rightAddress, OnRightPalmReceived);
    }

    

    void OnRightPalmReceived(OSCMessage message)
    {
        if (message.Values.Count >= 3)
        {
            float x = message.Values[0].FloatValue;
            float y = message.Values[1].FloatValue;
            bool grab = message.Values[2].FloatValue > 0.5f;

            PalmDataManager.RightPalm = PalmDataManager.ConvertNormalizedToCanvas(
                new Vector2(x, y), canvasRect);
            PalmDataManager.RightGrabbing = grab;
        }
    }

    void OnLeftPalmReceived(OSCMessage message)
    {
        if (message.Values.Count >= 3)
        {
            float x = message.Values[0].FloatValue;
            float y = message.Values[1].FloatValue;
            bool grab = message.Values[2].FloatValue > 0.5f;

            PalmDataManager.LeftPalm = PalmDataManager.ConvertNormalizedToCanvas(
                new Vector2(x, y), canvasRect);
            PalmDataManager.LeftGrabbing = grab;
        }
    }

}
