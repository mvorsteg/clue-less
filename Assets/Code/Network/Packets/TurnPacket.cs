using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

public struct TurnPacket : INetworkPacket
{
    public MessageIDs ID { get; private set; }
    public int turn;
    public TurnAction action;

    public TurnPacket(int turn, TurnAction action)
    {
        ID = MessageIDs.Turn_ToClient;
        this.turn = turn;
        this.action = action;
    }

    public TurnPacket(byte[] buffer)
    {
        int idx = 0;
        ID = (MessageIDs)IPAddress.NetworkToHostOrder(BitConverter.ToInt32(buffer, idx));
        idx += sizeof(Int32);
        turn = IPAddress.NetworkToHostOrder(BitConverter.ToInt32(buffer, idx));
        idx += sizeof(Int32);
        action = (TurnAction)IPAddress.NetworkToHostOrder(BitConverter.ToInt32(buffer, idx));
    }

    public byte[] GetBytes()
    {
        byte[] buffer = new byte[sizeof(Int32) + sizeof(Int32) + sizeof(Int32)];
        int idx = 0;

        byte[] tempBytes;

        tempBytes = BitConverter.GetBytes(IPAddress.HostToNetworkOrder((int)ID));
        tempBytes.CopyTo(buffer, idx);
        idx += sizeof(Int32);
        tempBytes = BitConverter.GetBytes(IPAddress.HostToNetworkOrder(turn));
        tempBytes.CopyTo(buffer, idx);
        idx += sizeof(Int32);
        tempBytes = BitConverter.GetBytes(IPAddress.HostToNetworkOrder((int)action));
        tempBytes.CopyTo(buffer, idx);

        return buffer;
    }
}