using System;
using System.Net;
using System.Text;

public struct MoveToRoomPacket : INetworkPacket
{
    public MessageIDs ID { get; private set; }
    public RoomType room;

    public MoveToRoomPacket(bool isToServer, RoomType room)
    {
        ID = isToServer ? MessageIDs.MoveToRoom_ToServer : MessageIDs.MoveToRoom_ToServer;
        this.room = room;
    }

    public MoveToRoomPacket(byte[] buffer)
    {
        int idx = 0;
        ID = (MessageIDs)IPAddress.NetworkToHostOrder(BitConverter.ToInt32(buffer, idx));
        idx += sizeof(Int32);
        room = (RoomType)IPAddress.NetworkToHostOrder(BitConverter.ToInt32(buffer, idx));
    }

    public byte[] GetBytes()
    {
        byte[] buffer = new byte[sizeof(Int32) + sizeof(Int32)];
        int idx = 0;

        byte[] tempBytes;

        tempBytes = BitConverter.GetBytes(IPAddress.HostToNetworkOrder((int)ID));
        tempBytes.CopyTo(buffer, idx);
        idx += sizeof(Int32);
        tempBytes = BitConverter.GetBytes(IPAddress.HostToNetworkOrder((int)room));
        tempBytes.CopyTo(buffer, idx);

        return buffer;
    }
}