using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

public class ClientNetworkInterface : BaseNetworkInterface
{
    private TcpClient tcpClient;
    private NetworkEndpoint netPlayer;
    private GuestEngine guestEngine;

    public ClientNetworkInterface(IPAddress ipAddress, int portNum) : base (ipAddress, portNum)
    {
        tcpClient = new TcpClient();
        isConnected = true;
        guestEngine = GameObject.FindAnyObjectByType<GuestEngine>();  // temp, not how we're keeping this
    }

    public override void Initialize()
    {
        // create a new thread to handle network client
        Thread connectThread = new Thread(ConnectToServer);
        connectThread.Start();
    }

    public override void ShutDown()
    {
        base.ShutDown();
        tcpClient.Dispose();
    }

    public void ConnectToServer()
    {
        Log("Starting connect thread");
        try
        {
            // attempt to connect to server
            Log(String.Format("attempting to connect to {0} : {1}", ipAddress.ToString(), portNum));
            tcpClient.Connect(ipAddress, portNum);
            Log("Connection successful");
            netPlayer = new NetworkEndpoint(tcpClient, this);
            networkStream = tcpClient.GetStream();
        }
        catch (SocketException e)
        {
            Log(e.Message);
        }
        catch (Exception e)
        {
            Log(String.Format("Unexpected error: {2}\n{2}\n{3}", e.Message, e.InnerException, e.StackTrace));
        }

        // send initial connect message so server knows our name
        ConnectRequestPacket pkt = new ConnectRequestPacket(processName);
        netPlayer.SendMessage(pkt);

        // begin processing messages from server 
        if (tcpClient.Connected && isConnected)
        {
            netPlayer.GetMessage();
        }
    }

    public void SendMessage(INetworkPacket pkt)
    {
        netPlayer.SendMessage(pkt);
    }

    public override void ProcessMessage(int clientID, MessageIDs messageID, byte[] buffer)
    {
        Log(String.Format("Processing message {0}", messageID.ToString()));
        switch(messageID)
        {
            case MessageIDs.Connect_ToClient :
            {
                ConnectResponsePacket pkt = new ConnectResponsePacket(buffer);
                // only accept if uninitialized
                if (netPlayer.id < 0)
                {
                    if (pkt.isAccepted)
                    {
                        guestEngine.AssignFromServer(pkt.assignedId, pkt.assignedCharacter);
                        netPlayer.id = pkt.assignedId;
                        Log(String.Format("Joined server as {0} (Assigned ID is {1})", pkt.assignedCharacter, pkt.assignedId));

                        // record all other players
                        foreach (Tuple<int, string, CharacterType> playerInfo in pkt.otherPlayers)
                        {
                            if (playerInfo.Item1 >= 0)
                            {
                                guestEngine.AddPlayer(playerInfo.Item1, playerInfo.Item2, playerInfo.Item3);
                                Log(String.Format("{0} is playing as {1} (Assigned ID is {2})", playerInfo.Item2, playerInfo.Item3.ToString(), playerInfo.Item1));
                            }
                        }
                        // temp
                        console.id = pkt.assignedId;
                    }
                    else
                    {
                        Log(String.Format("Rejected by server"));
                    }
                }
                break;
            }
            case MessageIDs.ConnectForward_ToClient :
            {
                ConnectForwardPacket pkt = new ConnectForwardPacket(buffer);
                guestEngine.AddPlayer(pkt.assignedId, pkt.userName, pkt.assignedCharacter);
                Log(String.Format("{0} Joined server as {1} (Assigned ID is {2})", pkt.userName, pkt.assignedCharacter, pkt.assignedId));
                break;
            }
            case MessageIDs.Disconnect_ToClient :

                break;
            case MessageIDs.Chat_ToClient :
            {
                ChatPacket pkt = new ChatPacket(buffer);
                if (pkt.senderID != clientID)
                {
                    Log(String.Format("{0} sent chat \"{1}\"", guestEngine.GetPlayerName(pkt.senderID), pkt.message));
                }
                break;
            }
            case MessageIDs.GameStart_ToClient :

                break;
            case MessageIDs.CharUpdate_ToClient :
            {
                CharUpdatePacket pkt = new CharUpdatePacket(buffer);
                if (pkt.userID == clientID)
                {
                    Log(String.Format("Changed character to {0}", pkt.character.ToString()));
                }
                else
                {
                    Log(String.Format("{0} changed character to {1}", guestEngine.GetPlayerName(pkt.userID), pkt.character.ToString()));
                }
                break;
            }
            case MessageIDs.MoveToRoom_ToClient :
            {
                MoveToRoomPacket pkt = new MoveToRoomPacket(buffer);
                if (pkt.userID == clientID)
                {
                    Log(String.Format("Moved to {0}", clientID, pkt.room.ToString()));
                }
                else
                {
                    Log(String.Format("{0} requested to move to {1}", guestEngine.GetPlayerName(pkt.userID), pkt.room.ToString()));
                }
                break;
            }
            case MessageIDs.Guess_ToClient :
            {
                GuessPacket pkt = new GuessPacket(buffer);
                if (pkt.userID == clientID)
                {
                    Log(String.Format("Guessed {0} used the {1} in the {2}", pkt.character.ToString(), pkt.weapon.ToString(), pkt.room.ToString()));
                }
                else
                {
                    Log(String.Format("{0} guessed {1} used the {2} in the {3}", guestEngine.GetPlayerName(pkt.userID), pkt.character.ToString(), pkt.weapon.ToString(), pkt.room.ToString()));
                }
                break;
            }
            case MessageIDs.Reveal_ToClient :

                break;
            case MessageIDs.Win_ToClient :

                break;
            case MessageIDs.Lost_ToClient :

                break;
            default :

                break;

        }
    }

}