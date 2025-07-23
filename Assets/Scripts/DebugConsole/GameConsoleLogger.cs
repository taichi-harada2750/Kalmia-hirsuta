using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameConsoleLogger : MonoBehaviour
{
    public TextMeshProUGUI logTextUI;
    public int maxLines = 50;
    private Queue<string> logQueue = new Queue<string>();

    void OnEnable()
    {
        Application.logMessageReceived += HandleLog;
    }

    void OnDisable()
    {
        Application.logMessageReceived -= HandleLog;
    }

    void HandleLog(string logString, string stackTrace, LogType type)
    {
        string color = type switch
        {
            LogType.Error => "red",
            LogType.Warning => "yellow",
            _ => "white"
        };

        string log = $"<color={color}>[{Time.time:F1}] {logString}</color>";
        logQueue.Enqueue(log);
        while (logQueue.Count > maxLines)
            logQueue.Dequeue();

        logTextUI.text = string.Join("\n", logQueue);
    }
}
