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
    private int maxPlayers;
    private TcpListener tcpListener;
    private ConcurrentDictionary<int, NetworkEndpoint> connectedPlayers;
    private HostEngine hostEngine;

    public ServerNetworkInterface(IPAddress ipAddress, int portNum, int maxClients) : base(ipAddress, portNum)
    {
        this.maxPlayers = maxClients;

        // keep list (dict) of TCP players that are currently connected
        connectedPlayers = new ConcurrentDictionary<int, NetworkEndpoint>();
        for (int i = 0; i < maxPlayers; i++)
        {
            connectedPlayers.TryAdd(i, null);
        }
    }

    public override void Initialize(BaseEngine engine, ConsoleLogger logger, string processName)
    {
        base.Initialize(engine, logger, processName);
        hostEngine = (HostEngine)engine;
        
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

    public void DisconnectPlayer(int id)
    {
        connectedPlayers[id] = null;
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
                    Thread clientListenThread = new Thread(netPlayer.GetMessage);
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
                }
                
                break;
            }
            case MessageIDs.Disconnect_ToServer :
            {
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
                if (hostEngine.MovePlayer(clientID, pkt.room))
                {
                    MoveToRoomPacket outPkt = new MoveToRoomPacket(false, clientID, pkt.room);
                    Broadcast(NetworkConstants.BROADCAST_ALL_CLIENTS, outPkt);
                }
                
                break;
            }
            case MessageIDs.Guess_ToServer :
            {
                GuessPacket pkt = new GuessPacket(buffer);
                Log(String.Format("Client{0} guessed {1} used the {2} in the {3}", clientID, pkt.character.ToString(), pkt.weapon.ToString(), pkt.room.ToString()));
                GuessPacket outPkt = new GuessPacket(false, clientID, pkt.isFinalGuess, pkt.character, pkt.weapon, pkt.room);
                Broadcast(NetworkConstants.BROADCAST_ALL_CLIENTS, outPkt);
                break;
            }
            case MessageIDs.Reveal_ToServer :
            {
                break;
            }
            default :
            {
                break;
            }

        }
    }
}