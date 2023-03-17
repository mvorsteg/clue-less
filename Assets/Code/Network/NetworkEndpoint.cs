using System;
using System.Collections.Concurrent;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text;

public class NetworkEndpoint
{
    public int id;

    public TcpClient tcpClient;
    NetworkStream networkStream;
    BaseNetworkInterface netInterface;

    public NetworkEndpoint(TcpClient tcpClient, BaseNetworkInterface netInterface)
    {
        this.tcpClient = tcpClient;
        this.netInterface = netInterface;
        networkStream = tcpClient.GetStream();
    }

    public void ShutDown()
    {
        networkStream.Close(1);
        tcpClient.Close();
    }

    public void SendMessage(INetworkPacket pkt)
    {
        netInterface.Log(String.Format("Sending {0}", pkt.ID.ToString()));
        // send size first
        byte[] buffer = pkt.GetBytes();
        int netInt = IPAddress.HostToNetworkOrder(buffer.Length);
        byte[] intBuff = BitConverter.GetBytes(netInt);
        networkStream.Write(intBuff, 0, 4);
        // then send actual message
        networkStream.Write(buffer);
    }

    public virtual void GetMessage()
    {
        //netInterface.Log(String.Format("Getting messages"));
        while (tcpClient.Connected)
        {
            try
            {
                byte[] sizeBuff = new byte[4];
                int bytesRead = 0;
                //netInterface.Log("Getting msg size...");
                while (bytesRead < 4)
                {
                    int newBytesRead = networkStream.Read(sizeBuff, bytesRead, 4 - bytesRead);
                    if (newBytesRead <= 0)
                    {
                        tcpClient.Close();
                        break;
                    }
                    bytesRead += newBytesRead;
                }

                if (tcpClient.Connected)
                {
                    int netMessageSize = BitConverter.ToInt32(sizeBuff);
                    int messageSize = IPAddress.NetworkToHostOrder(netMessageSize);
                    //netInterface.Log(String.Format("Message size is {0}", messageSize));

                    byte[] messageBuff = new byte[messageSize];
                    bytesRead = 0;

                    while (bytesRead < messageSize)
                    {
                        int newBytesRead = networkStream.Read(messageBuff, bytesRead, messageSize - bytesRead);
                        if (newBytesRead <= 0)
                        {
                            tcpClient.Close();
                            break;
                        }
                        bytesRead +=  newBytesRead;
                    }

                    if (tcpClient.Connected)
                    {
                        MessageIDs messageId = (MessageIDs)IPAddress.NetworkToHostOrder(BitConverter.ToInt32(messageBuff, 0));
                        netInterface.ProcessMessage(id, messageId, messageBuff);
                    }
                }
                else
                {
                    netInterface.Log("Server disconnected");
                }
            }
            catch (IOException e)
            {
                netInterface.Log(e.Message);
            }
        }
    }
     
}