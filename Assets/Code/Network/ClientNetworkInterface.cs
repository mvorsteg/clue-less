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

    private bool connectFailure = false;

    protected override void Awake()
    {
        base.Awake();
        tcpClient = new TcpClient();   
    }

    protected override void Update()
    {
        base.Update();
        // check to see if server failure
        if (connectFailure)
        {
            guestEngine.ErrorOut("Failed to connect to server.\nIs it running?");
        }
    }

    public override void Initialize(IPAddress ipAddress, int portNum, BaseEngine engine, ConsoleLogger logger, string processName)
    {
        base.Initialize(ipAddress, portNum, engine, logger, processName);
        guestEngine = (GuestEngine)engine;
        isConnected = true;
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
        bool connectSuccess = false;
        Log("Starting connect thread");
        try
        {
            // attempt to connect to server
            Log(String.Format("attempting to connect to {0} : {1}", ipAddress.ToString(), portNum));
            tcpClient.Connect(ipAddress, portNum);
            Log("Connection successful");
            netPlayer = new NetworkEndpoint(tcpClient, this);
            networkStream = tcpClient.GetStream();
            connectSuccess = true;
        }
        catch (SocketException e)
        {
            Log(e.Message);
        }
        catch (Exception e)
        {
            Log(String.Format("Unexpected error: {0}\n{1}\n{2}", e.Message, e.InnerException, e.StackTrace));
        }
        if (connectSuccess)
        {
            // send initial connect message so server knows our name
            ConnectRequestPacket pkt = new ConnectRequestPacket(processName);
            netPlayer.SendMessage(pkt);

            // begin processing messages from server 
            if (tcpClient.Connected && isConnected)
            {
                netPlayer.ContinuouslyGetMessages();
            }
        }
        else
        {
            connectFailure = true;
            Log("Failed to connect to server");
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
                        guestEngine.AssignFromServer(pkt.assignedId, processName, pkt.assignedCharacter);
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
                        guestEngine.ErrorOut("Rejected by server.\nIt may be full or a game may be in session already.");
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
            case MessageIDs.Ready_ToClient :
            {
                ReadyPacket pkt = new ReadyPacket(buffer);
                guestEngine.SetPlayerReady(pkt.userID, pkt.ready);
                break;
            }
            case MessageIDs.Disconnect_ToClient :
            {
                DisconnectPacket pkt = new DisconnectPacket(buffer);
                guestEngine.RemovePlayer(pkt.userID);
                break;
            }
            case MessageIDs.GameStart_ToClient :
            {
                GameStartPacket pkt = new GameStartPacket(buffer);
                // if for us, take cards
                if (pkt.userID == netPlayer.id)
                {
                    guestEngine.AssignClueCards(pkt.characterClues, pkt.weaponClues, pkt.roomClues);
                    guestEngine.StartGame();
                }
                break;
            }
            case MessageIDs.Turn_ToClient :
            {
                TurnPacket pkt = new TurnPacket(buffer);
                guestEngine.SetTurn(pkt.turn, pkt.action);
                break;
            }
            case MessageIDs.Chat_ToClient :
            {
                ChatPacket pkt = new ChatPacket(buffer);
                if (pkt.senderID != clientID)
                {
                    guestEngine.ProcessChatMessage(pkt.senderID, pkt.message);
                    Log(String.Format("{0} sent chat \"{1}\"", guestEngine.GetPlayerName(pkt.senderID), pkt.message));
                }
                break;
            }
            case MessageIDs.CharUpdate_ToClient :
            {
                CharUpdatePacket pkt = new CharUpdatePacket(buffer);
                guestEngine.UpdateCharacter(pkt.userID, pkt.character);
                break;
            }
            case MessageIDs.MoveToRoom_ToClient :
            {
                MoveToRoomPacket pkt = new MoveToRoomPacket(buffer);
                guestEngine.MovePlayer(pkt.userID, pkt.room, pkt.isForcedMove);
                break;
            }
            case MessageIDs.Guess_ToClient :
            {
                GuessPacket pkt = new GuessPacket(buffer);
                guestEngine.Guess(pkt.userID, pkt.isFinalGuess, pkt.character, pkt.weapon, pkt.room);
                break;
            }
            case MessageIDs.Reveal_ToClient :
            {
                RevealPacket pkt = new RevealPacket(buffer);
                guestEngine.EnqueueReveal(pkt.sendID, pkt.recvID, pkt.clueType, pkt.character, pkt.weapon, pkt.room);
                break;
            }
            case MessageIDs.WinLose_ToClient :
            {
                GameOverPacket pkt = new GameOverPacket(buffer);
                if (pkt.type == GameOverType.Win)
                {
                    guestEngine.Win(pkt.userID);
                }
                else if (pkt.type == GameOverType.Lose)
                {
                    guestEngine.Lose(pkt.userID);
                }
                else if (pkt.type == GameOverType.Error)
                {
                    guestEngine.ErrorOut("Unfortunately a communication error has occurred.\nThe game will not be able to continue.\nPlease ensure everyone playing has an internet connection.");
                }
                else
                {
                    Log(String.Format("Unrecognized GameOverType {0}", pkt.type));
                }
                break;
            }
            default :

                break;

        }
    }

}