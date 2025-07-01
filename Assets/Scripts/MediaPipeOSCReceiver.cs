using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

public class MediaPipeOSCReceiver : MonoBehaviour
{
    UdpClient udpClient;
    Thread receiveThread;
    int listenPort = 9000;

    void Start()
    {
        Debug.Log("MediaPipeOSCReceiver started.");
        udpClient = new UdpClient(listenPort);
        receiveThread = new Thread(new ThreadStart(ReceiveData));
        receiveThread.IsBackground = true;
        receiveThread.Start();
    }

    void ReceiveData()
    {
        IPEndPoint remoteEndPoint = new IPEndPoint(IPAddress.Any, listenPort);

        try
        {
            while (true)
            {
                byte[] data = udpClient.Receive(ref remoteEndPoint);
                string text = Encoding.UTF8.GetString(data);

                if (text.StartsWith("/hand/right_palm") || text.StartsWith("/hand/left_palm"))
                {
                    string[] parts = text.Split(' ');
                    if (parts.Length >= 4 &&
                        float.TryParse(parts[1], out float x) &&
                        float.TryParse(parts[2], out float y) &&
                        float.TryParse(parts[3], out float grab))
                    {
                        float scale = 500f;
                        Vector3 pos = new Vector3(x * scale, y * scale, 0);
                        bool isGrabbing = grab > 0.5f;

                        if (text.StartsWith("/hand/right_palm"))
                        {
                            PalmDataManager.RightPalm = pos;
                            PalmDataManager.RightGrabbing = isGrabbing;
                        }
                        else
                        {
                            PalmDataManager.LeftPalm = pos;
                            PalmDataManager.LeftGrabbing = isGrabbing;
                        }

                        Debug.Log($"Received: {text} → Position: {pos}, Grab: {isGrabbing}");
                    }
                    else
                    {
                        Debug.LogWarning("Parse failed or insufficient data.");
                    }
                }
                else
                {
                    Debug.Log("Received: " + text);  // その他のメッセージはそのまま表示
                }


                if (text.StartsWith("/hand/right_palm") || text.StartsWith("/hand/left_palm"))
                {
                    string[] parts = text.Split(' ');
                    if (parts.Length >= 4 &&
                        float.TryParse(parts[1], out float x) &&
                        float.TryParse(parts[2], out float y) &&
                        float.TryParse(parts[3], out float grab))
                    {
                        float scale = 500f;
                        Vector3 pos = new Vector3(x * scale, y * scale, 0);
                        bool isGrabbing = grab > 0.5f;

                        if (text.StartsWith("/hand/right_palm"))
                        {
                            PalmDataManager.RightPalm = pos;
                            PalmDataManager.RightGrabbing = isGrabbing;
                        }
                        else if (text.StartsWith("/hand/left_palm"))
                        {
                            PalmDataManager.LeftPalm = pos;
                            PalmDataManager.LeftGrabbing = isGrabbing;
                        }

                        Debug.Log($"[{text}] Pos: {pos}, Grab: {isGrabbing}");
                    }
                }

                            }
                        }
                        catch (SocketException ex)
                        {
                            Debug.Log("Socket closed: " + ex.Message);
                        }
                    }

    void OnApplicationQuit()
    {
        if (udpClient != null)
        {
            udpClient.Close();
        }
        if (receiveThread != null)
        {
            receiveThread.Abort();
        }
    }
}
