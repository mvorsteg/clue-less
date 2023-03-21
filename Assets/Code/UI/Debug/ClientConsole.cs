using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

// takes text in and publishes it as a chat message from a client
// really just a debug feature
public class ClientConsole : MonoBehaviour
{
    public InputField inputField;
    public BaseNetworkInterface netInterface;
    public GameObject newCommandPrefab;
    public Transform consoleParent;
    private Queue<string> messageQueue = new Queue<string>();
    public int id = -1;

    private void Update()
    {
        if (messageQueue.Count > 0)
        {
            LogMessageToVirtualConsole(messageQueue.Dequeue());
        }
    }

    public void SubmitCommand()
    {
        string rawCmd = inputField.text;
        string[] tokens = rawCmd.Split(" ");
        string cmdType = tokens[0];
        if (netInterface is ClientNetworkInterface)
        {
            ClientNetworkInterface clientInterface = (ClientNetworkInterface)netInterface;
            switch (cmdType.ToLower())
            {
                case ("connect"):
                {
                    string nameStr = tokens[1];
                    netInterface.processName = nameStr;
                    netInterface.Initialize();
                    break;
                }
                case ("char"):
                {
                    string charStr = tokens[1];
                    if (Enum.TryParse<CharacterType>(charStr, true, out CharacterType charEnum))
                    {
                        CharUpdatePacket pkt = new CharUpdatePacket(true, id, charEnum);
                        clientInterface.SendMessage(pkt);
                    }
                    break;
                }
                case ("chat"):
                {
                    string text = string.Join(" ", tokens.Skip(1).ToList<string>());
                    QueueMessageForDisplay(text);
                    ChatPacket pkt = new ChatPacket(true, id, text);
                    clientInterface.SendMessage(pkt);
                    break;
                }
                case ("move"):
                {
                    string roomStr = tokens[1];
                    if (Enum.TryParse<RoomType>(roomStr, true, out RoomType roomEnum))
                    {
                        MoveToRoomPacket pkt = new MoveToRoomPacket(true, id, roomEnum);
                        clientInterface.SendMessage(pkt);
                    }
                    break;
                }
                case ("guess"):
                {
                    string charStr = tokens[1];
                    string weaponStr = tokens[2];
                    string roomStr = tokens[3];
                    if (Enum.TryParse<CharacterType>(charStr, true, out CharacterType charEnum) &&
                        Enum.TryParse<WeaponType>(weaponStr, true, out WeaponType weaponEnum) &&
                        Enum.TryParse<RoomType>(roomStr, true, out RoomType roomEnum))
                    {
                        GuessPacket pkt = new GuessPacket(true, id, false, charEnum, weaponEnum, roomEnum);
                        clientInterface.SendMessage(pkt);
                    }
                    break;
                }
                case ("accusation"):
                {
                    string charStr = tokens[1];
                    string weaponStr = tokens[2];
                    string roomStr = tokens[3];
                    if (Enum.TryParse<CharacterType>(charStr, true, out CharacterType charEnum) &&
                        Enum.TryParse<WeaponType>(weaponStr, true, out WeaponType weaponEnum) &&
                        Enum.TryParse<RoomType>(roomStr, true, out RoomType roomEnum))
                    {
                        GuessPacket pkt = new GuessPacket(true, id, true, charEnum, weaponEnum, roomEnum);
                        clientInterface.SendMessage(pkt);
                    }
                    break;
                }
            }
        }
        
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