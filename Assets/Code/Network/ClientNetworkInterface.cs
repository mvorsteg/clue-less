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

    public override void Initialize(BaseEngine engine, ConsoleLogger logger, string processName)
    {
        base.Initialize(engine, logger, processName);
        guestEngine = (GuestEngine)engine;
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

                        // record all other players
                        foreach (Tuple<int, string, CharacterType> playerInfo in pkt.otherPlayers)
                        {
                            if (playerInfo.Item1 >= 0)
                            {
                                guestEngine.AddPlayer(playerInfo.Item1, playerInfo.Item2, playerInfo.Item3);
                            }
                        }
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
                guestEngine.UpdateCharacter(pkt.userID, pkt.character);
                break;
            }
            case MessageIDs.MoveToRoom_ToClient :
            {
                MoveToRoomPacket pkt = new MoveToRoomPacket(buffer);
                guestEngine.MovePlayer(pkt.userID, pkt.room);
                break;
            }
            case MessageIDs.Guess_ToClient :
            {
                GuessPacket pkt = new GuessPacket(buffer);
                guestEngine.Guess(pkt.userID, pkt.isFinalGuess, pkt.character, pkt.weapon, pkt.room);
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