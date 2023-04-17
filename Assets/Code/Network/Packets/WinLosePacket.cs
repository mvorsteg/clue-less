using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

public struct WinLosePacket : INetworkPacket
{
    public MessageIDs ID { get; private set; }
    public int userID;
    public bool win;

    public WinLosePacket(int userID, bool win)
    {
        ID = MessageIDs.WinLose_ToClient;
        this.userID = userID;
        this.win = win;
    }

    public WinLosePacket(byte[] buffer)
    {
        int idx = 0;
        ID = (MessageIDs)IPAddress.NetworkToHostOrder(BitConverter.ToInt32(buffer, idx));
        idx += sizeof(Int32);
        userID = IPAddress.NetworkToHostOrder(BitConverter.ToInt32(buffer, idx));
        idx += sizeof(Int32);
        win = BitConverter.ToBoolean(buffer, idx);
        
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
        tempBytes = BitConverter.GetBytes(win);
        tempBytes.CopyTo(buffer, idx);

        return buffer;
    }
}