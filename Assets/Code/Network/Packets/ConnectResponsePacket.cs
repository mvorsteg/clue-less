using System;
using System.Net;

public struct ConnectResponsePacket : INetworkPacket
{
    public MessageIDs ID { get; private set; }
    public bool isAccepted;
    public int assignedId;

    public ConnectResponsePacket(bool isAccepted, int assignedId)
    {
        ID = MessageIDs.Connect_ToClient;
        this.isAccepted = isAccepted;
        this.assignedId = assignedId;
    }

    public ConnectResponsePacket(byte[] buffer)
    {
        int idx = 0;
        ID = (MessageIDs)IPAddress.NetworkToHostOrder(BitConverter.ToInt32(buffer, idx));
        idx += sizeof(Int32);
        isAccepted = BitConverter.ToBoolean(buffer, idx);
        idx += sizeof(bool);
        assignedId = IPAddress.NetworkToHostOrder(BitConverter.ToInt32(buffer, idx));
    }

    public byte[] GetBytes()
    {
        byte[] buffer = new byte[sizeof(Int32) + NetworkConstants.MAX_USER_NAME_LEN * sizeof(Char)];
        int idx = 0;

        byte[] tempBytes;

        tempBytes = BitConverter.GetBytes(IPAddress.HostToNetworkOrder((int)ID));
        tempBytes.CopyTo(buffer, idx);
        idx += sizeof(Int32);
        tempBytes = BitConverter.GetBytes(isAccepted);
        tempBytes.CopyTo(buffer, idx);
        idx += sizeof(Int32);
        tempBytes = BitConverter.GetBytes(IPAddress.HostToNetworkOrder(assignedId));
        tempBytes.CopyTo(buffer, idx);
        idx += sizeof(Int32);

        return buffer;
    }
}