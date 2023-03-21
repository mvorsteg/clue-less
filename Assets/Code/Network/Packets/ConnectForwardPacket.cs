using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

public struct ConnectForwardPacket : INetworkPacket
{
    public MessageIDs ID { get; private set; }
    public string userName;
    public int assignedId;
    public CharacterType assignedCharacter;

    public ConnectForwardPacket(string userName, int assignedId, CharacterType assignedCharacter)
    {
        ID = MessageIDs.ConnectForward_ToClient;
        this.userName = userName;
        this.assignedId = assignedId;
        this.assignedCharacter = assignedCharacter;
    }

    public ConnectForwardPacket(byte[] buffer)
    {
        int idx = 0;
        ID = (MessageIDs)IPAddress.NetworkToHostOrder(BitConverter.ToInt32(buffer, idx));
        idx += sizeof(Int32);
        userName = Encoding.ASCII.GetString(buffer, idx, NetworkConstants.MAX_USER_NAME_LEN).TrimEnd((Char)0);
        idx += sizeof(char) * NetworkConstants.MAX_USER_NAME_LEN;
        assignedId = IPAddress.NetworkToHostOrder(BitConverter.ToInt32(buffer, idx));
        idx += sizeof(Int32);
        assignedCharacter = (CharacterType)IPAddress.NetworkToHostOrder(BitConverter.ToInt32(buffer, idx));
    }

    public byte[] GetBytes()
    {
        byte[] buffer = new byte[sizeof(Int32) + sizeof(Int32) + sizeof(char) * NetworkConstants.MAX_USER_NAME_LEN + sizeof(Int32)];
        int idx = 0;

        byte[] tempBytes;

        tempBytes = BitConverter.GetBytes(IPAddress.HostToNetworkOrder((int)ID));
        tempBytes.CopyTo(buffer, idx);
        idx += sizeof(Int32);
        tempBytes = Encoding.ASCII.GetBytes(userName);
        tempBytes.CopyTo(buffer, idx);
        idx += sizeof(char) * NetworkConstants.MAX_USER_NAME_LEN;
        tempBytes = BitConverter.GetBytes(IPAddress.HostToNetworkOrder(assignedId));
        tempBytes.CopyTo(buffer, idx);
        idx += sizeof(Int32);
        tempBytes = BitConverter.GetBytes(IPAddress.HostToNetworkOrder((int)assignedCharacter));
        tempBytes.CopyTo(buffer, idx);

        return buffer;
    }
}