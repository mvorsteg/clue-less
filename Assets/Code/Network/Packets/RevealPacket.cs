using System;
using System.Net;
using System.Text;

public struct RevealPacket : INetworkPacket
{
    public MessageIDs ID { get; private set; }
    public int sendID;
    public int recvID;
    public ClueType clueType;
    public CharacterType character;
    public WeaponType weapon;
    public RoomType room;

    public RevealPacket(bool isToServer, int sendID, int recvID, ClueType clueType, CharacterType character, WeaponType weapon, RoomType room)
    {
        ID = isToServer ? MessageIDs.Reveal_ToServer : MessageIDs.Reveal_ToClient;
        this.sendID = sendID;
        this.recvID = recvID;
        this.clueType = clueType;
        this.character = character;
        this.weapon = weapon;
        this.room = room;
    }

    public RevealPacket(byte[] buffer)
    {
        int idx = 0;
        ID = (MessageIDs)IPAddress.NetworkToHostOrder(BitConverter.ToInt32(buffer, idx));
        idx += sizeof(Int32);
        sendID = IPAddress.NetworkToHostOrder(BitConverter.ToInt32(buffer, idx));
        idx += sizeof(Int32);
        recvID = IPAddress.NetworkToHostOrder(BitConverter.ToInt32(buffer, idx));
        idx += sizeof(Int32);
        clueType = (ClueType)IPAddress.NetworkToHostOrder(BitConverter.ToInt32(buffer, idx));
        idx += sizeof(Int32);
        character = (CharacterType)IPAddress.NetworkToHostOrder(BitConverter.ToInt32(buffer, idx));
        idx += sizeof(Int32);
        weapon = (WeaponType)IPAddress.NetworkToHostOrder(BitConverter.ToInt32(buffer, idx));
        idx += sizeof(Int32);
        room = (RoomType)IPAddress.NetworkToHostOrder(BitConverter.ToInt32(buffer, idx));
        idx += sizeof(Int32);
    }

    public byte[] GetBytes()
    {
        byte[] buffer = new byte[sizeof(Int32) + sizeof(Int32) + sizeof(Int32) + sizeof(Int32) + sizeof(Int32) + sizeof(Int32) + sizeof(Int32)];
        int idx = 0;

        byte[] tempBytes;

        tempBytes = BitConverter.GetBytes(IPAddress.HostToNetworkOrder((int)ID));
        tempBytes.CopyTo(buffer, idx);
        idx += sizeof(Int32);
        tempBytes = BitConverter.GetBytes(IPAddress.HostToNetworkOrder(sendID));
        tempBytes.CopyTo(buffer, idx);
        idx += sizeof(Int32);
        tempBytes = BitConverter.GetBytes(IPAddress.HostToNetworkOrder(recvID));
        tempBytes.CopyTo(buffer, idx);
        idx += sizeof(Int32);
        tempBytes = BitConverter.GetBytes(IPAddress.HostToNetworkOrder((int)clueType));
        tempBytes.CopyTo(buffer, idx);
        idx += sizeof(Int32);
        tempBytes = BitConverter.GetBytes(IPAddress.HostToNetworkOrder((int)character));
        tempBytes.CopyTo(buffer, idx);
        idx += sizeof(Int32);
        tempBytes = BitConverter.GetBytes(IPAddress.HostToNetworkOrder((int)weapon));
        tempBytes.CopyTo(buffer, idx);
        idx += sizeof(Int32);
        tempBytes = BitConverter.GetBytes(IPAddress.HostToNetworkOrder((int)room));
        tempBytes.CopyTo(buffer, idx);
        idx += sizeof(Int32);

        return buffer;
    }
}