using System;
using System.Net;
using System.Text;

public struct GuessPacket : INetworkPacket
{
    public MessageIDs ID { get; private set; }
    public bool isFinalGuess;
    public CharacterType character;
    public WeaponType weapon;
    public RoomType room;

    public GuessPacket(bool isToServer, bool isFinalGuess, CharacterType character, WeaponType weapon, RoomType room)
    {
        ID = isToServer ? MessageIDs.Guess_ToServer : MessageIDs.Guess_ToClient;
        this.isFinalGuess = isFinalGuess;
        this.character = character;
        this.weapon = weapon;
        this.room = room;
    }

    public GuessPacket(byte[] buffer)
    {
        int idx = 0;
        ID = (MessageIDs)IPAddress.NetworkToHostOrder(BitConverter.ToInt32(buffer, idx));
        idx += sizeof(Int32);
        isFinalGuess = BitConverter.ToBoolean(buffer, idx);
        idx += sizeof(Boolean);
        character = (CharacterType)IPAddress.NetworkToHostOrder(BitConverter.ToInt32(buffer, idx));
        idx += sizeof(Int32);
        weapon = (WeaponType)IPAddress.NetworkToHostOrder(BitConverter.ToInt32(buffer, idx));
        idx += sizeof(Int32);
        room = (RoomType)IPAddress.NetworkToHostOrder(BitConverter.ToInt32(buffer, idx));
        idx += sizeof(Int32);
    }

    public byte[] GetBytes()
    {
        byte[] buffer = new byte[sizeof(Int32) + NetworkConstants.MAX_USER_NAME_LEN * sizeof(Char)];
        int idx = 0;

        byte[] tempBytes;

        tempBytes = BitConverter.GetBytes(IPAddress.HostToNetworkOrder((int)ID));
        tempBytes.CopyTo(buffer, idx);
        idx += sizeof(Int32);
        tempBytes = BitConverter.GetBytes(isFinalGuess);
        tempBytes.CopyTo(buffer, idx);
        idx += sizeof(Boolean);
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