using System;
using System.Collections.Generic;
using System.Net;

public struct GameStartPacket : INetworkPacket
{
    public MessageIDs ID { get; private set; }
    public int userID;
    public int turn;
    public List<CharacterType> characterClues;
    public List<WeaponType> weaponClues;
    public List<RoomType> roomClues;

    public GameStartPacket(bool isToServer, int userID)
    {
        ID = isToServer ? MessageIDs.GameStart_ToServer : MessageIDs.GameStart_ToClient;
        this.userID = userID;
        this.turn = 0;
        this.characterClues = new List<CharacterType>();
        this.weaponClues = new List<WeaponType>();
        this.roomClues = new List<RoomType>();
    }
    public GameStartPacket(bool isToServer, int userID, int turn, List<CharacterType> characterClues, List<WeaponType> weaponClues, List<RoomType> roomClues)
    {
        ID = isToServer ? MessageIDs.GameStart_ToServer : MessageIDs.GameStart_ToClient;
        this.userID = userID;
        this.turn = turn;
        this.characterClues = characterClues;
        this.weaponClues = weaponClues;
        this.roomClues = roomClues;
    }

    public GameStartPacket(byte[] buffer)
    {
        int idx = 0;
        ID = (MessageIDs)IPAddress.NetworkToHostOrder(BitConverter.ToInt32(buffer, idx));
        idx += sizeof(Int32);
        userID = IPAddress.NetworkToHostOrder(BitConverter.ToInt32(buffer, idx));
        idx += sizeof(Int32);
        turn = IPAddress.NetworkToHostOrder(BitConverter.ToInt32(buffer, idx));
        idx += sizeof(Int32);
        characterClues = new List<CharacterType>();
        weaponClues = new List<WeaponType>();
        roomClues = new List<RoomType>();
        for (int i = 0; i < NetworkConstants.MAX_CARDS_PER_PLAYER; i++)
        {
            int otherClue = IPAddress.NetworkToHostOrder(BitConverter.ToInt32(buffer, idx));
            idx += sizeof(Int32);
            if (otherClue >= 0)
            {
                characterClues.Add((CharacterType)otherClue);
            }
        }
        for (int i = 0; i < NetworkConstants.MAX_CARDS_PER_PLAYER; i++)
        {
            int otherClue = IPAddress.NetworkToHostOrder(BitConverter.ToInt32(buffer, idx));
            idx += sizeof(Int32);
            if (otherClue >= 0)
            {
                weaponClues.Add((WeaponType)otherClue);
            }
        }
        for (int i = 0; i < NetworkConstants.MAX_CARDS_PER_PLAYER; i++)
        {
            int otherClue = IPAddress.NetworkToHostOrder(BitConverter.ToInt32(buffer, idx));
            idx += sizeof(Int32);
            if (otherClue >= 0)
            {
                roomClues.Add((RoomType)otherClue);
            }
        }
        idx += sizeof(Int32);
    }

    public byte[] GetBytes()
    {
        byte[] buffer = new byte[sizeof(Int32) + sizeof(Int32) + sizeof(Int32) + 3 * NetworkConstants.MAX_CARDS_PER_PLAYER * sizeof(Int32)];
        int idx = 0;

        byte[] tempBytes;

        tempBytes = BitConverter.GetBytes(IPAddress.HostToNetworkOrder((int)ID));
        tempBytes.CopyTo(buffer, idx);
        idx += sizeof(Int32);
        tempBytes = BitConverter.GetBytes(IPAddress.HostToNetworkOrder(userID));
        tempBytes.CopyTo(buffer, idx);
        idx += sizeof(Int32);
        tempBytes = BitConverter.GetBytes(IPAddress.HostToNetworkOrder(turn));
        tempBytes.CopyTo(buffer, idx);
        idx += sizeof(Int32);
        int i;
        for (i = 0; i < Math.Min(characterClues.Count, NetworkConstants.MAX_CARDS_PER_PLAYER); i++)
        {
            tempBytes = BitConverter.GetBytes(IPAddress.HostToNetworkOrder((int)characterClues[i]));
            tempBytes.CopyTo(buffer, idx);
            idx += sizeof(Int32);
        }
        for (; i < NetworkConstants.MAX_CARDS_PER_PLAYER; i++)
        {
            tempBytes = BitConverter.GetBytes(IPAddress.HostToNetworkOrder(NetworkConstants.INVALID_CLUE_TYPE));
            tempBytes.CopyTo(buffer, idx);
            idx += sizeof(Int32);
        }
        for (i = 0; i < Math.Min(weaponClues.Count, NetworkConstants.MAX_CARDS_PER_PLAYER); i++)
        {
            tempBytes = BitConverter.GetBytes(IPAddress.HostToNetworkOrder((int)weaponClues[i]));
            tempBytes.CopyTo(buffer, idx);
            idx += sizeof(Int32);
        }
        for (; i < NetworkConstants.MAX_CARDS_PER_PLAYER; i++)
        {
            tempBytes = BitConverter.GetBytes(IPAddress.HostToNetworkOrder(NetworkConstants.INVALID_CLUE_TYPE));
            tempBytes.CopyTo(buffer, idx);
            idx += sizeof(Int32);
        }
        for (i = 0; i < Math.Min(roomClues.Count, NetworkConstants.MAX_CARDS_PER_PLAYER); i++)
        {
            tempBytes = BitConverter.GetBytes(IPAddress.HostToNetworkOrder((int)roomClues[i]));
            tempBytes.CopyTo(buffer, idx);
            idx += sizeof(Int32);
        }
        for (; i < NetworkConstants.MAX_CARDS_PER_PLAYER; i++)
        {
            tempBytes = BitConverter.GetBytes(IPAddress.HostToNetworkOrder(NetworkConstants.INVALID_CLUE_TYPE));
            tempBytes.CopyTo(buffer, idx);
            idx += sizeof(Int32);
        }

        return buffer;
    }
}