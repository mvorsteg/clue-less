using System;
using System.Net;
using System.Text;

public struct ChatPacket : INetworkPacket
{
    public MessageIDs ID { get; private set; }
    public int senderID;
    public string message;

    public ChatPacket(bool isToServer, int senderID, string message)
    {
        ID = isToServer ? MessageIDs.Chat_ToServer : MessageIDs.Chat_ToClient;
        this.senderID = senderID;
        this.message = message;
    }

    public ChatPacket(byte[] buffer)
    {
        int idx = 0;
        int messageID32 = IPAddress.NetworkToHostOrder(BitConverter.ToInt32(buffer, idx));
        ID = (MessageIDs)messageID32;
        idx += sizeof(Int32);
        senderID = IPAddress.NetworkToHostOrder(BitConverter.ToInt32(buffer, idx));
        idx += sizeof(Int32);
        message = Encoding.ASCII.GetString(buffer, idx, NetworkConstants.MAX_CHAT_MSG_LEN);
        message = message.TrimEnd((Char)0);
    }
    public byte[] GetBytes()
    {
        byte[] buffer = new byte[sizeof(Int32) + sizeof(Int32) + NetworkConstants.MAX_CHAT_MSG_LEN * sizeof(Char)];
        int idx = 0;

        byte[] tempBytes;

        tempBytes = BitConverter.GetBytes(IPAddress.HostToNetworkOrder((int)ID));
        tempBytes.CopyTo(buffer, idx);
        idx += sizeof(Int32);
        tempBytes = BitConverter.GetBytes(IPAddress.HostToNetworkOrder(senderID));
        tempBytes.CopyTo(buffer, idx);
        idx += sizeof(Int32);
        tempBytes = Encoding.ASCII.GetBytes(message);
        tempBytes.CopyTo(buffer, idx);

        return buffer;
    }
}