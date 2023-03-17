using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

public class ClientNetworkInterface : BaseNetworkInterface
{
    private TcpClient tcpClient;
    private NetworkEndpoint netPlayer;

    public ClientNetworkInterface(IPAddress ipAddress, int portNum) : base (ipAddress, portNum)
    {
        tcpClient = new TcpClient();
        isConnected = true;
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
        ConnectRequestPacket pkt = new ConnectRequestPacket("TestUserName");
        netPlayer.SendMessage(pkt);

        // begin processing messages from server 
        if (tcpClient.Connected && isConnected)
        {
            netPlayer.GetMessage();
        }
    }

    public void SendChatMessage(string msg)
    {
        ChatPacket pkt = new ChatPacket(true, netPlayer.id, msg);
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
                Log(String.Format("Joined server"));
                break;
            }
            case MessageIDs.Disconnect_ToClient :

                break;
            case MessageIDs.Chat_ToClient :
            {
                ChatPacket pkt = new ChatPacket(buffer);
                Log(String.Format("Client{0} sent chat \"{1}\"", clientID, pkt.message));
                break;
            }
            case MessageIDs.GameStart_ToClient :

                break;
            case MessageIDs.CharUpdate_ToClient :

                break;
            case MessageIDs.MoveToRoom_ToClient :

                break;
            case MessageIDs.Guess_ToClient :

                break;
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