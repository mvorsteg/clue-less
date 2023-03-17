public interface INetworkPacket
{
    MessageIDs ID { get; }
    byte[] GetBytes();
}