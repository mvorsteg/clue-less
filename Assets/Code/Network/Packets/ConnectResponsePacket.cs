using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

public struct ConnectResponsePacket : INetworkPacket
{
    public MessageIDs ID { get; private set; }
    public bool isAccepted;
    public int assignedId;
    public CharacterType assignedCharacter;
    public List<Tuple<int, string, CharacterType>> otherPlayers;

    public ConnectResponsePacket(bool isAccepted, int assignedId, CharacterType assignedCharacter, List<Tuple<int, string, CharacterType>> otherPlayers)
    {
        ID = MessageIDs.Connect_ToClient;
        this.isAccepted = isAccepted;
        this.assignedId = assignedId;
        this.assignedCharacter = assignedCharacter;
        this.otherPlayers = otherPlayers;
    }

    public ConnectResponsePacket(byte[] buffer)
    {
        int idx = 0;
        ID = (MessageIDs)IPAddress.NetworkToHostOrder(BitConverter.ToInt32(buffer, idx));
        idx += sizeof(Int32);
        isAccepted = BitConverter.ToBoolean(buffer, idx);
        idx += sizeof(bool);
        assignedId = IPAddress.NetworkToHostOrder(BitConverter.ToInt32(buffer, idx));
        idx += sizeof(Int32);
        assignedCharacter = (CharacterType)IPAddress.NetworkToHostOrder(BitConverter.ToInt32(buffer, idx));
        idx += sizeof(Int32);
        otherPlayers = new List<Tuple<int, string, CharacterType>>(NetworkConstants.MAX_NUM_PLAYERS);

        for (int i = 0; i < NetworkConstants.MAX_NUM_PLAYERS; i++)
        {
            int otherID = IPAddress.NetworkToHostOrder(BitConverter.ToInt32(buffer, idx));
            idx += sizeof(Int32);
            string otherName = Encoding.ASCII.GetString(buffer, idx, NetworkConstants.MAX_USER_NAME_LEN).TrimEnd((Char)0);
            idx += sizeof(char) * NetworkConstants.MAX_USER_NAME_LEN;
            CharacterType otherType = (CharacterType)IPAddress.NetworkToHostOrder(BitConverter.ToInt32(buffer, idx));
            idx += sizeof(Int32);

            if (otherID >= 0)
            {
                otherPlayers.Add(new Tuple<int, string, CharacterType>(otherID, otherName, otherType));
            }
        }
    }

    public byte[] GetBytes()
    {
        byte[] buffer = new byte[sizeof(Int32) + sizeof(Boolean) + sizeof(Int32) + sizeof(Int32) +
                                (sizeof(Int32) + sizeof(char) * NetworkConstants.MAX_USER_NAME_LEN + sizeof(Int32)) * NetworkConstants.MAX_NUM_PLAYERS];
        int idx = 0;

        byte[] tempBytes;

        tempBytes = BitConverter.GetBytes(IPAddress.HostToNetworkOrder((int)ID));
        tempBytes.CopyTo(buffer, idx);
        idx += sizeof(Int32);
        tempBytes = BitConverter.GetBytes(isAccepted);
        tempBytes.CopyTo(buffer, idx);
        idx += sizeof(Boolean);
        tempBytes = BitConverter.GetBytes(IPAddress.HostToNetworkOrder(assignedId));
        tempBytes.CopyTo(buffer, idx);
        idx += sizeof(Int32);
        tempBytes = BitConverter.GetBytes(IPAddress.HostToNetworkOrder((int)assignedCharacter));
        tempBytes.CopyTo(buffer, idx);
        idx += sizeof(Int32);

        int i = 0;
        if (otherPlayers != null)
        {
            for (; i < otherPlayers.Count; i++)
            {
                tempBytes = BitConverter.GetBytes(IPAddress.HostToNetworkOrder(otherPlayers[i].Item1));
                tempBytes.CopyTo(buffer, idx);
                idx += sizeof(Int32);
                tempBytes = Encoding.ASCII.GetBytes(otherPlayers[i].Item2);
                tempBytes.CopyTo(buffer, idx);
                idx += sizeof(char) * NetworkConstants.MAX_USER_NAME_LEN;
                tempBytes = BitConverter.GetBytes(IPAddress.HostToNetworkOrder((int)otherPlayers[i].Item3));
                tempBytes.CopyTo(buffer, idx);
                idx += sizeof(Int32);
            }
        }
        for (; i < NetworkConstants.MAX_NUM_PLAYERS; i++)
        {
            tempBytes = BitConverter.GetBytes(IPAddress.HostToNetworkOrder(NetworkConstants.SERVER_ID));
            tempBytes.CopyTo(buffer, idx);
            idx += sizeof(Int32);
            idx += sizeof(char) * NetworkConstants.MAX_USER_NAME_LEN;
            idx += sizeof(Int32);
        }

        return buffer;
    }
}