using UnityEngine;
using UnityEngine.UI;

public class BoardRoom : MonoBehaviour
{
    public RoomType type;
    public GameObject[] characterIcons;
    private ClientNetworkInterface netInterface;
    private GuestEngine engine;

    public bool Interactable
    {
        set
        {
            GetComponent<Button>().interactable = value;
        }
    }

    private void Awake()
    {
        
    }

    private void Start()
    {
        netInterface = FindObjectOfType<ClientNetworkInterface>();
        engine = FindObjectOfType<GuestEngine>();

        foreach (GameObject icon in characterIcons)
        {
            icon.SetActive(false);
        }
        
    }

    public void OnRoomClicked()
    {
        MoveToRoomPacket pkt = new MoveToRoomPacket(true, engine.ID, type);
        netInterface.SendMessage(pkt);
    }

    public void SetCharacterInRoom(CharacterType character, bool value)
    {
        Debug.Log(character.ToString() + " " + (int)character + " | size " + characterIcons.Length);
        characterIcons[(int)character].SetActive(value);
    }
}