using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InGameConsole : MonoBehaviour
{
    public int maxLines = 10;
    public TextMeshProUGUI consoleText;
    private Queue<string> logLines = new Queue<string>();

    void Awake()
    {
        Application.logMessageReceived += HandleLog;
    }

    void OnDestroy()
    {
        Application.logMessageReceived -= HandleLog;
    }

    void HandleLog(string logString, string stackTrace, LogType type)
    {
        AddMessageToConsole(logString);
    }

    public void AddMessageToConsole(string message)
    {
        if (logLines.Count >= maxLines)
        {
            logLines.Dequeue();
        }

        logLines.Enqueue(message);
        consoleText.text = string.Join("\n", logLines.ToArray());
    }
}
