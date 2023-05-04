using System;
using System.Net;
using System.Text;

public struct MoveToRoomPacket : INetworkPacket
{
    public MessageIDs ID { get; private set; }
    public int userID;
    public RoomType room;
    public bool isForcedMove;

    public MoveToRoomPacket(bool isToServer, int userID, RoomType room, bool isForcedMove)
    {
        ID = isToServer ? MessageIDs.MoveToRoom_ToServer : MessageIDs.MoveToRoom_ToClient;
        this.userID = userID;
        this.room = room;
        this.isForcedMove = isForcedMove;
    }

    public MoveToRoomPacket(byte[] buffer)
    {
        int idx = 0;
        ID = (MessageIDs)IPAddress.NetworkToHostOrder(BitConverter.ToInt32(buffer, idx));
        idx += sizeof(Int32);
        userID = IPAddress.NetworkToHostOrder(BitConverter.ToInt32(buffer, idx));
        idx += sizeof(Int32);
        room = (RoomType)IPAddress.NetworkToHostOrder(BitConverter.ToInt32(buffer, idx));
        idx += sizeof(Int32);
        isForcedMove = BitConverter.ToBoolean(buffer, idx);
    }

    public byte[] GetBytes()
    {
        byte[] buffer = new byte[sizeof(Int32) + sizeof(Int32) + sizeof(Int32) + sizeof(Boolean)];
        int idx = 0;

        byte[] tempBytes;

        tempBytes = BitConverter.GetBytes(IPAddress.HostToNetworkOrder((int)ID));
        tempBytes.CopyTo(buffer, idx);
        idx += sizeof(Int32);
        tempBytes = BitConverter.GetBytes(IPAddress.HostToNetworkOrder(userID));
        tempBytes.CopyTo(buffer, idx);
        idx += sizeof(Int32);
        tempBytes = BitConverter.GetBytes(IPAddress.HostToNetworkOrder((int)room));
        tempBytes.CopyTo(buffer, idx);
        idx += sizeof(Int32);
        tempBytes = BitConverter.GetBytes((isForcedMove));
        tempBytes.CopyTo(buffer, idx);

        return buffer;
    }
}