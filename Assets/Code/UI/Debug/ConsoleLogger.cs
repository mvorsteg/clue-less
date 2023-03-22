using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConsoleLogger : MonoBehaviour
{
    private Queue<string> messageQueue = new Queue<string>();

    public GameObject newCommandPrefab;
    public Transform consoleParent;

    private void Update()
    {
        if (messageQueue.Count > 0)
        {
            LogMessageToVirtualConsole(messageQueue.Dequeue());
        }
    }

    public void Log(string message, SubsystemType type)
    {
        string formattedMessage = string.Format("[{0}] {1}", type.ToString(), message);
        messageQueue.Enqueue(formattedMessage);
    }


    public void LogMessageToVirtualConsole(string message)
    {
        Text newMessageText = Instantiate(newCommandPrefab, consoleParent).GetComponent<Text>();
        newMessageText.text = message;
    }

}