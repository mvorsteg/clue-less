using UnityEngine;
using UnityEngine.UI;

public class TurnUI : MonoBehaviour
{
    public GameObject endTurnObj;
    private ClientNetworkInterface netInterface;
    private GuestEngine engine;
    private void Start()
    {
        netInterface = FindObjectOfType<ClientNetworkInterface>();
        engine = FindObjectOfType<GuestEngine>();
        endTurnObj.SetActive(false);
    }

    public void PromptEndTurn()
    {
        endTurnObj.SetActive(true);
    }

    public void EndTurn()
    {
        TurnDonePacket pkt = new TurnDonePacket(engine.ID);
        netInterface.SendMessage(pkt);
        endTurnObj.SetActive(false);
    }
}