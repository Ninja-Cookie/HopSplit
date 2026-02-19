using System;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace HopSplit.LiveSplit
{
    internal static class LiveSplitManager
    {
        private     const   string          IP          = "127.0.0.1";
        private     const   int             PORT        = 16834;
        private     const   float           RefreshRate = 2f;
        internal    static  NetworkStream   Stream;

        private static CancellationTokenSource CancelRefresh;

        internal delegate void Connected();
        internal delegate void Disconnected();
        internal delegate void StatusChanged(Status status);

        internal static event Connected        OnConnected;
        internal static event Disconnected     OnDisconnected;
        internal static event StatusChanged    OnStatusChanged;

        internal enum Status
        {
            Disconnected,
            Disconnecting,
            Connected,
            Connecting
        }

        private static Status _connectionStatus = Status.Disconnected;
        internal static Status ConnectionStatus
        {
            get => _connectionStatus;
            private set
            {
                bool changed = false;

                if (changed = (value != _connectionStatus))
                {
                    switch (value)
                    {
                        case Status.Connected:
                            OnConnected?.Invoke();
                        break;

                        case Status.Connecting:
                        break;

                        case Status.Disconnected:
                            OnDisconnected?.Invoke();
                        break;
                    }
                }

                if (value == Status.Disconnected)
                    Stream = null;
                _connectionStatus = value;

                if (changed)
                    OnStatusChanged?.Invoke(_connectionStatus);
            }
        }

        internal static async void Connect()
        {
            if (ConnectionStatus != Status.Disconnected)
                return;

            ConnectionStatus = Status.Connecting;

            using (var client = new TcpClient())
            {
                if (TryConnect(client))
                {
                    using (Stream = client.GetStream())
                    {
                        CancelRefresh       = new CancellationTokenSource();
                        ConnectionStatus    = Status.Connected;

                        while (ConnectionStatus == Status.Connected)
                        {
                            try { await Task.Delay(TimeSpan.FromSeconds(RefreshRate), CancelRefresh.Token); } catch { break; }
                            if (ConnectionStatus != Status.Connected || !(await TryPingClientAsync(client)))
                                break;
                        }
                    }
                }
            }

            ConnectionStatus = Status.Disconnected;
        }

        internal static void Disconnect()
        {
            if (ConnectionStatus == Status.Connected)
            {
                ConnectionStatus = Status.Disconnecting;
                CancelRefresh?.Cancel();
            }
        }

        private static bool TryConnect(TcpClient client)
        {
            if (client == null)
                return false;

            try
            {
                client.Connect(IP, PORT);
                return TryPingClient(client);
            }
            catch
            {
                return false;
            }
        }

        private static async Task<bool> TryConnectAsync(TcpClient client)
        {
            if (client == null)
                return false;

            try
            {
                await client.ConnectAsync(IP, PORT);
                return await TryPingClientAsync(client);
            }
            catch
            {
                return false;
            }
        }

        private static async Task<bool> TryPingClientAsync(TcpClient client)
        {
            return await TryPingClient(client, true);
        }

        private static bool TryPingClient(TcpClient client)
        {
            return TryPingClient(client, false).GetAwaiter().GetResult();
        }

        private static async Task<bool> TryPingClient(TcpClient client, bool isAsync)
        {
            if (client == null || client.Client == null)
                return false;

            try
            {
                if (client.Client.Poll(0, SelectMode.SelectRead) && client.Client.Available == 0)
                {
                    byte[] buffer = new byte[1];
                    if (isAsync)
                        return await client.Client.ReceiveAsync(new ArraySegment<byte>(buffer), SocketFlags.Peek) != 0;
                    else
                        return client.Client.Receive(buffer, SocketFlags.Peek) != 0;
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
