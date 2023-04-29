using System;
using System.Net;
using System.Text;

public struct ReadyPacket : INetworkPacket
{
    public MessageIDs ID { get; private set; }
    public int userID;
    public bool ready;

    public ReadyPacket(bool isToServer, int userID, bool ready)
    {
        ID = isToServer ? MessageIDs.Ready_ToServer : MessageIDs.Ready_ToClient;
        this.userID = userID;
        this.ready = ready;
    }

    public ReadyPacket(byte[] buffer)
    {
        int idx = 0;
        ID = (MessageIDs)IPAddress.NetworkToHostOrder(BitConverter.ToInt32(buffer, idx));
        idx += sizeof(Int32);
        userID = IPAddress.NetworkToHostOrder(BitConverter.ToInt32(buffer, idx));
        idx += sizeof(Int32);
        ready = BitConverter.ToBoolean(buffer, idx);
    }

    public byte[] GetBytes()
    {
        byte[] buffer = new byte[sizeof(Int32) + sizeof(Int32) + sizeof(Boolean)];
        int idx = 0;

        byte[] tempBytes;

        tempBytes = BitConverter.GetBytes(IPAddress.HostToNetworkOrder((int)ID));
        tempBytes.CopyTo(buffer, idx);
        idx += sizeof(Int32);
        tempBytes = BitConverter.GetBytes(IPAddress.HostToNetworkOrder(userID));
        tempBytes.CopyTo(buffer, idx);
        idx += sizeof(Int32);
        tempBytes = BitConverter.GetBytes(ready);
        tempBytes.CopyTo(buffer, idx);

        return buffer;
    }
}