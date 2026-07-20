using Lidgren.Network;

namespace Pong.Net.Server
{
    public static class NetServerManager
    {
        private static bool _isHosted;

        private const int RefreshRatePerSecond = 60;
        private static readonly Dictionary<NetConnection, Client> _clients = [];

        private static NetServer _server;
        private static Thread _netThread;

        private static readonly object _syncObject = new();
        private static bool _shouldStopThread;

        private static uint _time;

        public static void Host()
        {
            if (_isHosted)
                return;

            _clients.Clear();
            _time = 0;
            _isHosted = true;

            var config = new NetPeerConfiguration("basic")
            {
                Port = 2000,
                PingInterval = 2,
                ConnectionTimeout = 30
            };

            _server = new NetServer(config);
            _server.Start();
            Console.WriteLine($"Server started on port {_server.Port}");

            _netThread = new Thread(Listen);
            _netThread.IsBackground = true;
            _netThread.Start();
        }

        public static void StopIfRunning()
        {
            if (!_isHosted)
                return;

            _isHosted = false;

            lock (_syncObject)
            {
                _shouldStopThread = true;
            }

            while (_shouldStopThread) { }
            _server.Shutdown("Server closed.");
        }

        private static void Listen()
        {
            while (true)
            {
                NetIncomingMessage message;
                while ((message = _server.ReadMessage()) != null)
                {
                    ProcessMessage(message);
                }

                _server.Recycle(message);
                int sleepMs = (int)(1f / RefreshRatePerSecond * 1000f);
                Thread.Sleep(sleepMs);

                lock (_syncObject)
                {
                    if (_shouldStopThread)
                    {
                        _shouldStopThread = false;
                        break;
                    }
                }

                _time += (uint)sleepMs;
            }
        }

        private static void ProcessMessage(NetIncomingMessage message)
        {
            if (message.MessageType == NetIncomingMessageType.StatusChanged)
            {
                if (message.SenderConnection.Status == NetConnectionStatus.Connected)
                {
                    HandleConnectionPacket(message);
                }
                else if (message.SenderConnection.Status == NetConnectionStatus.Disconnected)
                {
                    HandleDisconnectionPacket(message);
                }
            }
            else if (message.MessageType == NetIncomingMessageType.Data)
            {
                HandleDataPacket(message);
            }
        }

        private static void HandleConnectionPacket(NetIncomingMessage message)
        {
            string name = message.SenderConnection.RemoteHailMessage.ReadString();
            int x = message.SenderConnection.RemoteHailMessage.ReadInt32();
            int y = message.SenderConnection.RemoteHailMessage.ReadInt32();

            // Check if name is taken
            bool nameTaken = false;
            foreach (var pair in _clients)
            {
                if (pair.Value.Name == name)
                {
                    nameTaken = true;
                    break;
                }
            }

            if (nameTaken)
            {
                NetOutgoingMessage declineMsg = _server.CreateMessage();
                declineMsg.Write((byte)PacketType.ConnectionDeclined_NameIsTaken);
                _server.SendMessage(declineMsg, message.SenderConnection, NetDeliveryMethod.ReliableOrdered);

                Console.WriteLine($"{name}'s (IP: {message.SenderConnection.RemoteEndPoint.Address}, UUID: {message.SenderConnection.RemoteUniqueIdentifier} join request has been denied (name was taken)");
            }
            else
            {
                NetOutgoingMessage approvedMsg = _server.CreateMessage();
                approvedMsg.Write((byte)PacketType.ConnectionApproved);
                _server.SendMessage(approvedMsg, message.SenderConnection, NetDeliveryMethod.ReliableOrdered);

                NetOutgoingMessage connectionMsg = _server.CreateMessage();
                connectionMsg.Write((byte)PacketType.Connection);
                connectionMsg.Write(message.SenderConnection.RemoteUniqueIdentifier);
                connectionMsg.Write(name);
                connectionMsg.Write(x);
                connectionMsg.Write(y);

                _server.SendToAll(connectionMsg, NetDeliveryMethod.ReliableOrdered);

                _clients[message.SenderConnection] = new Client(message.SenderConnection.RemoteUniqueIdentifier, name, x, y);
                Console.WriteLine($"{name} (IP: {message.SenderConnection.RemoteEndPoint.Address}, UUID: {message.SenderConnection.RemoteUniqueIdentifier}) has joined the server");
            }
        }

        private static void HandleDisconnectionPacket(NetIncomingMessage message)
        {
            if (!_clients.TryGetValue(message.SenderConnection, out Client client))
                return;

            NetOutgoingMessage msg = _server.CreateMessage();
            msg.Write((byte)PacketType.Disconnection);
            msg.Write(client.Id);
            _server.SendToAll(msg, NetDeliveryMethod.ReliableOrdered);

            _clients.Remove(message.SenderConnection);
            Console.WriteLine($"{client.Name} (IP: {message.SenderConnection.RemoteEndPoint.Address.ToString()}, UUID: {message.SenderConnection.RemoteUniqueIdentifier}) has left the server");
        }

        private static void HandleDataPacket(NetIncomingMessage message)
        {
            PacketType type = (PacketType)message.ReadByte();
            if (type == PacketType.RequestOtherPlayers)
            {
                NetOutgoingMessage otherPlayersMsg = _server.CreateMessage();
                otherPlayersMsg.Write((byte)PacketType.OtherPlayers);
                otherPlayersMsg.Write(_clients.Count);

                foreach (var pair in _clients)
                {
                    otherPlayersMsg.Write(pair.Value.Id);
                    otherPlayersMsg.Write(pair.Value.Name);
                    otherPlayersMsg.Write(pair.Value.SpawnX);
                    otherPlayersMsg.Write(pair.Value.SpawnY);
                }

                _server.SendMessage(otherPlayersMsg, message.SenderConnection, NetDeliveryMethod.ReliableOrdered);
            }
            else if (type == PacketType.PlayerUpdate)
            {
                int x = message.ReadInt32();
                int y = message.ReadInt32();

                NetOutgoingMessage updateMsg = _server.CreateMessage();
                updateMsg.Write((byte)PacketType.PlayerUpdate);
                updateMsg.Write(message.SenderConnection.RemoteUniqueIdentifier);
                updateMsg.Write(x);
                updateMsg.Write(y);

                _server.SendToAll(updateMsg, NetDeliveryMethod.UnreliableSequenced);
            }
        }
    }
}
