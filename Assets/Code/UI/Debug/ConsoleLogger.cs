using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConsoleLogger : MonoBehaviour
{
    private Queue<Tuple<string, bool>> messageQueue = new Queue<Tuple<string, bool>>();

    public GameObject newCommandPrefab;
    public Transform consoleParent;
    private bool showDebugMessages = false;
    private List<GameObject> debugMessages;

    private void Awake()
    {
        debugMessages = new List<GameObject>();
    }

    private void Update()
    {
        if (messageQueue.Count > 0)
        {
            Tuple<string, bool> messageTuple = messageQueue.Dequeue();
            LogMessageToVirtualConsole(messageTuple.Item1, messageTuple.Item2);
        }
    }

    public void Log(string message, SubsystemType type)
    {
        string formattedMessage = string.Format("[{0}] {1}", type.ToString(), message);
        messageQueue.Enqueue(new Tuple<string, bool>(formattedMessage, true));
    }

    public void LogChat(string message)
    {
        messageQueue.Enqueue(new Tuple<string, bool>(message, false));
    }

    public void LogMessageToVirtualConsole(string message, bool debug)
    {
        Text newMessageText = Instantiate(newCommandPrefab, consoleParent).GetComponent<Text>();
        newMessageText.text = message;
        if (debug)
        {
            debugMessages.Add(newMessageText.gameObject);
            newMessageText.gameObject.SetActive(showDebugMessages);
        }
    }

    public void ShowDebugMessages(bool value)
    {
        showDebugMessages = value;
        if (debugMessages != null)
        {
            foreach (GameObject debugObj in debugMessages)
            {
                debugObj.SetActive(value);
            }
        }
    }

}