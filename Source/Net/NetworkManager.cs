using Lidgren.Network;

namespace Pong.Net
{
    public static class NetworkManager
    {
        public static long Id => _client.UniqueIdentifier;
        private static NetClient _client;

        public static void Connect(string ip, string name, int x, int y)
        {
            _client?.Shutdown("");

            NetPeerConfiguration config = new("basic");
            _client = new NetClient(config);
            _client.Start();

            NetOutgoingMessage hailMessage = CreateMessage();
            hailMessage.Write(name);
            hailMessage.Write(x);
            hailMessage.Write(y);

            _client.Connect(ip, 2000, hailMessage);
        }

        public static void Disconnect()
        {
            _client?.Shutdown("disconnect");
        }

        public static NetOutgoingMessage CreateMessage()
        {
            return _client.CreateMessage();
        }

        public static NetSendResult SendMessage(NetOutgoingMessage message, NetDeliveryMethod method)
        {
            return _client.SendMessage(message, method);
        }

        public static List<NetIncomingMessage> ReadMessages()
        {
            List<NetIncomingMessage> messages = [];
            _client.ReadMessages(messages);
            return messages;
        }
    }
}
