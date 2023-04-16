using UnityEngine;

public class BoardRoom : MonoBehaviour
{
    public RoomType type;
    private ClientNetworkInterface netInterface;
    private GuestEngine engine;

    private void Start()
    {
        netInterface = FindObjectOfType<ClientNetworkInterface>();
        engine = FindObjectOfType<GuestEngine>();
    }

    public void OnRoomClicked()
    {
        MoveToRoomPacket pkt = new MoveToRoomPacket(true, engine.ID, type);
        netInterface.SendMessage(pkt);
    }
}