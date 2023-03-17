using UnityEngine;
using UnityEngine.UI;

// takes text in and publishes it as a chat message from a client
// really just a debug feature
public class ClientConsole : MonoBehaviour
{
    public InputField inputField;
    private ClientNetworkInterface clientInterface;
    public NetworkStart ns;

    private void Start()
    {
        clientInterface = (ClientNetworkInterface)(ns.netIFace); // VERY BAD PRACTICE BUT TEMPORARY
    }

    public void SubmitCommand()
    {
        string text = inputField.text;
        clientInterface.SendChatMessage(text);
    }
}