using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// takes text in and publishes it as a chat message from a client
// really just a debug feature
public class ClientConsole : MonoBehaviour
{
    public InputField inputField;
    private ClientNetworkInterface clientInterface;
    public NetworkStart ns;
    public GameObject newCommandPrefab;
    public Transform consoleParent;
    private Queue<string> messageQueue;

    private void Start()
    {
        messageQueue = new Queue<string>();
        ns.Initialize(this);
        clientInterface = (ClientNetworkInterface)(ns.netIFace); // VERY BAD PRACTICE BUT TEMPORARY
    }

    private void Update()
    {
        if (messageQueue.Count > 0)
        {
            LogMessageToVirtualConsole(messageQueue.Dequeue());
        }
    }

    public void SubmitCommand()
    {
        string text = inputField.text;
        clientInterface.SendChatMessage(text);
        QueueMessageForDisplay(text);
    }

    public void QueueMessageForDisplay(string message)
    {
        messageQueue.Enqueue(message);
    }

    public void LogMessageToVirtualConsole(string message)
    {
        Text newMessageText = Instantiate(newCommandPrefab, consoleParent).GetComponent<Text>();
        newMessageText.text = message;
    }
}