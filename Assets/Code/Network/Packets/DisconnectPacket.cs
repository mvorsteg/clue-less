using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

public struct DisconnectPacket : INetworkPacket
{
    public MessageIDs ID { get; private set; }
    public int userID;

    public DisconnectPacket(int userID)
    {
        ID = MessageIDs.Disconnect_ToClient;
        this.userID = userID;
    }

    public DisconnectPacket(byte[] buffer)
    {
        int idx = 0;
        ID = (MessageIDs)IPAddress.NetworkToHostOrder(BitConverter.ToInt32(buffer, idx));
        idx += sizeof(Int32);
        userID = IPAddress.NetworkToHostOrder(BitConverter.ToInt32(buffer, idx));
    }

    public byte[] GetBytes()
    {
        byte[] buffer = new byte[sizeof(Int32) + sizeof(Int32)];
        int idx = 0;

        byte[] tempBytes;

        tempBytes = BitConverter.GetBytes(IPAddress.HostToNetworkOrder((int)ID));
        tempBytes.CopyTo(buffer, idx);
        idx += sizeof(Int32);
        tempBytes = BitConverter.GetBytes(IPAddress.HostToNetworkOrder(userID));
        tempBytes.CopyTo(buffer, idx);

        return buffer;
    }
}