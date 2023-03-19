using System;
using System.Net;
using System.Text;

public struct CharUpdatePacket : INetworkPacket
{
    public MessageIDs ID { get; private set; }
    public CharacterType character;

    public CharUpdatePacket(bool isToServer, CharacterType character)
    {
        ID = isToServer ? MessageIDs.CharUpdate_ToServer : MessageIDs.CharUpdate_ToClient;
        this.character = character;
    }

    public CharUpdatePacket(byte[] buffer)
    {
        int idx = 0;
        ID = (MessageIDs)IPAddress.NetworkToHostOrder(BitConverter.ToInt32(buffer, idx));
        idx += sizeof(Int32);
        character = (CharacterType)IPAddress.NetworkToHostOrder(BitConverter.ToInt32(buffer, idx));
    }

    public byte[] GetBytes()
    {
        byte[] buffer = new byte[sizeof(Int32) + sizeof(Int32)];
        int idx = 0;

        byte[] tempBytes;

        tempBytes = BitConverter.GetBytes(IPAddress.HostToNetworkOrder((int)ID));
        tempBytes.CopyTo(buffer, idx);
        idx += sizeof(Int32);
        tempBytes = BitConverter.GetBytes(IPAddress.HostToNetworkOrder((int)character));
        tempBytes.CopyTo(buffer, idx);

        return buffer;
    }
}