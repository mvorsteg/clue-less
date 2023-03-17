using System;
using System.Net;
using System.Text;

public struct ConnectRequestPacket : INetworkPacket
{
    public MessageIDs ID { get; private set; }
    public string userName;

    public ConnectRequestPacket(string userName)
    {
        ID = MessageIDs.Connect_ToServer;
        this.userName = userName;
    }

    public ConnectRequestPacket(byte[] buffer)
    {
        int idx = 0;
        ID = (MessageIDs)IPAddress.NetworkToHostOrder(BitConverter.ToInt32(buffer, idx));
        idx += sizeof(Int32);
        userName = Encoding.ASCII.GetString(buffer, idx, NetworkConstants.MAX_USER_NAME_LEN);
    }

    public byte[] GetBytes()
    {
        byte[] buffer = new byte[sizeof(Int32) + NetworkConstants.MAX_USER_NAME_LEN * sizeof(Char)];
        int idx = 0;

        byte[] tempBytes;

        tempBytes = BitConverter.GetBytes(IPAddress.HostToNetworkOrder((int)ID));
        tempBytes.CopyTo(buffer, idx);
        idx += sizeof(Int32);
        tempBytes = Encoding.ASCII.GetBytes(userName);
        tempBytes.CopyTo(buffer, idx);

        return buffer;
    }
}