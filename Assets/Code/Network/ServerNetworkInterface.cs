using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

public class ServerNetworkInterface : BaseNetworkInterface
{
    public int maxPlayers;
    private TcpListener tcpListener;
    private ConcurrentDictionary<int, NetworkEndpoint> connectedPlayers;
    private HostEngine hostEngine;

    protected override void Awake()
    {
        base.Awake();
        // keep list (dict) of TCP players that are currently connected
        connectedPlayers = new ConcurrentDictionary<int, NetworkEndpoint>();
        for (int i = 0; i < maxPlayers; i++)
        {
            connectedPlayers.TryAdd(i, null);
        }
    }

    public override void Initialize(IPAddress ipAddress, int portNum, BaseEngine engine, ConsoleLogger logger, string processName)
    {
        base.Initialize(ipAddress, portNum, engine, logger, processName);
        hostEngine = (HostEngine)engine;
        logger.ShowDebugMessages(true);
        
        // listen for incoming connections on new thread
        tcpListener = new TcpListener(IPAddress.Any, portNum);
        tcpListener.Start();
        
        hostEngine = GameObject.FindAnyObjectByType<HostEngine>();  // temp, not 

        Thread listenThread = new Thread(ListenForConnections);
        listenThread.Start();

    }

    public override void ShutDown()
    {
        base.ShutDown();
        // stop taking new connections
        tcpListener.Stop();
        // forcibly shut down each connected player
        for (int i = 0; i < maxPlayers; i++)
        {
            if (connectedPlayers[i] != null)
            {
                Log(String.Format("Shutting down player {0}", i));
                connectedPlayers[i].ShutDown();
            }
        }
    }

    public override void DisconnectPlayer(int id)
    {
        connectedPlayers[id] = null;
        hostEngine.RemovePlayer(id);
    }

    public void SendMessage(int clientID, INetworkPacket pkt)
    {
        if (clientID >= 0)
        {
            connectedPlayers[clientID].SendMessage(pkt);
        }
    }
    
    public void Broadcast(int senderID, INetworkPacket pkt)
    {
        // forward received message to all players except the one who sent it
        for (int i = 0; i < maxPlayers; i++)
        {
            if (i != senderID && connectedPlayers[i] != null)
            {
                connectedPlayers[i].SendMessage(pkt);
            }
        }
    }

    void ListenForConnections()
    {
        Log(String.Format("Starting server @ {0} : {1}", ipAddress.ToString(), portNum));
        Log("Listening for connections...");
        isConnected = true;
        while (isConnected)
        {
            try
            {
                // blocks until new client is accepted- so this has to be its own thread
                TcpClient tcpClient = tcpListener.AcceptTcpClient();
                Log("Got 1 connection");
                NetworkEndpoint netPlayer = new NetworkEndpoint(tcpClient, this);

                bool playerAdded = false;

                for (int i = 0; i < maxPlayers; i++)
                {
                    //Log(String.Format("{0}, {1}", i, playersDict[i]));
                    if (connectedPlayers[i] == null)
                    {
                        // assign ID to player and pass relevant data
                        netPlayer.id = i;
                        connectedPlayers[i] = netPlayer;
                        playerAdded = true;
                        break;
                    }
                }

                if (playerAdded)
                {
                    // send initial connect message to new client
                    //ConnectResponsePacket pkt = new ConnectResponsePacket(true, netPlayer.id);
                    //netPlayer.SendMessage(pkt);
                    Thread clientListenThread = new Thread(netPlayer.ContinuouslyGetMessages);
                    clientListenThread.Start();
                }
                else
                {
                    // failed to add player, don't leave client hanging
                    ConnectResponsePacket pkt = new ConnectResponsePacket(false, -1, CharacterType.Mustard, null);
                    netPlayer.SendMessage(pkt);
                }
            }
            catch (SocketException e)
            {
                // expected - client disconnected
                Log(e.Message);
                isConnected = false;
            }
            catch (Exception e)
            {
                // unexpected
                Log(e.StackTrace);
                isConnected = false;
            }
        }
    }

    public override void ProcessMessage(int clientID, MessageIDs messageID, byte[] buffer)
    {
        Log(String.Format("Processing message {0}", messageID.ToString()));
        switch(messageID)
        {
            case MessageIDs.Connect_ToServer :
            {
                List<Tuple<int, string, CharacterType>> allPlayers = hostEngine.GetAllPlayerInfo();
                ConnectRequestPacket pkt = new ConnectRequestPacket(buffer);
                Log(String.Format("Client{0} requested to join server", clientID, pkt.userName));
                ConnectResponsePacket outPkt;
                if (hostEngine.AddPlayer(clientID, pkt.userName, out CharacterType assignedCharacter))
                {
                    outPkt = new ConnectResponsePacket(true, clientID, assignedCharacter, allPlayers);
                    if (connectedPlayers.TryGetValue(clientID, out NetworkEndpoint endpoint))
                    {
                        endpoint.SendMessage(outPkt);
                    }
                    ConnectForwardPacket fwdPacket = new ConnectForwardPacket(pkt.userName, clientID, assignedCharacter);
                    Broadcast(clientID, fwdPacket);
                }
                else
                {
                    outPkt = new ConnectResponsePacket(false, clientID, assignedCharacter, allPlayers);
                    SendMessage(clientID, outPkt);
                }
                
                break;
            }
            case MessageIDs.Disconnect_ToServer :
            {
                break;
            }
            case MessageIDs.Ready_ToServer :
            {
                ReadyPacket pkt = new ReadyPacket(buffer);
                hostEngine.SetPlayerReady(pkt.userID, pkt.ready);
                break;
            }
            case MessageIDs.GameStart_ToServer :
            {
                GameStartPacket pkt = new GameStartPacket(buffer);
                hostEngine.StartGame();
                break;
            }
            case MessageIDs.Chat_ToServer :
            {
                ChatPacket pkt = new ChatPacket(buffer);
                Log(String.Format("Client{0} sent chat \"{1}\"", clientID, pkt.message));
                ChatPacket outPkt = new ChatPacket(false, pkt.senderID, pkt.message);
                Broadcast(clientID, outPkt);
                break;
            }
            case MessageIDs.CharUpdate_ToServer :
            {
                CharUpdatePacket pkt = new CharUpdatePacket(buffer);
                Log(String.Format("Client{0} requested to change character to {1}", clientID, pkt.character.ToString()));
                if (hostEngine.UpdateCharacter(clientID, pkt.character))
                {
                    CharUpdatePacket outPkt = new CharUpdatePacket(false, clientID, pkt.character);
                    Broadcast(NetworkConstants.BROADCAST_ALL_CLIENTS, outPkt);
                }
                break;
            }
            case MessageIDs.MoveToRoom_ToServer :
            {
                MoveToRoomPacket pkt = new MoveToRoomPacket(buffer);
                Log(String.Format("Client{0} requested to move to {1}", clientID, pkt.room.ToString()));
                if (hostEngine.MovePlayer(clientID, pkt.room, false))
                {
                    // MoveToRoomPacket outPkt = new MoveToRoomPacket(false, clientID, pkt.room);
                    // Broadcast(NetworkConstants.BROADCAST_ALL_CLIENTS, outPkt);
                }
                
                break;
            }
            case MessageIDs.Guess_ToServer :
            {
                GuessPacket pkt = new GuessPacket(buffer);
                Log(String.Format("Client{0} guessed {1} used the {2} in the {3}", clientID, pkt.character.ToString(), pkt.weapon.ToString(), pkt.room.ToString()));
                if (hostEngine.Guess(pkt.userID, pkt.isFinalGuess, pkt.character, pkt.weapon, pkt.room))
                {
                    // GuessPacket outPkt = new GuessPacket(false, clientID, pkt.isFinalGuess, pkt.character, pkt.weapon, pkt.room);
                    // Broadcast(NetworkConstants.BROADCAST_ALL_CLIENTS, outPkt);
                }
                break;
            }
            case MessageIDs.Reveal_ToServer :
            {
                RevealPacket pkt = new RevealPacket(buffer);
                Log(String.Format("Client{0} revealed {1}", clientID, pkt.clueType == ClueType.Character ? pkt.character : pkt.clueType == ClueType.Weapon ? pkt.weapon : pkt.room));
                if (hostEngine.Reveal(pkt.sendID, pkt.recvID, pkt.clueType, pkt.character, pkt.weapon, pkt.room))
                {
                    // forward packet to the player whose turn it is
                    RevealPacket outPkt = new RevealPacket(false, pkt.sendID, pkt.recvID, pkt.clueType, pkt.character, pkt.weapon, pkt.room);
                    SendMessage(outPkt.recvID, outPkt);
                }
                break;
            }
            case MessageIDs.TurnDone_ToServer :
            {
                TurnDonePacket pkt = new TurnDonePacket(buffer);
                Log("Client{0} is done their turn");
                if (hostEngine.EndTurn(pkt.userID))
                {

                }
                break;
            }
            default :
            {
                break;
            }

        }
    }
}