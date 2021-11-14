using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Tcp
{
    internal class ServerListener
    {
        private readonly List<TcpClient> _connectedClients = new();
        private byte _delimiter = 0x13;
        private readonly List<TcpClient> _disconnectedClients = new();
        private readonly SimpleTcpServer _parent;
        private readonly List<byte> _queuedMsg = new();
        private Thread _rxThread;


        internal ServerListener(SimpleTcpServer parentServer, IPAddress ipAddress, int port)
        {
            QueueStop = false;
            _parent = parentServer;
            IPAddress = ipAddress;
            Port = port;
            ReadLoopIntervalMs = 10;

            Listener = new TcpListenerEx(ipAddress, port);
            Listener.Start();

            ThreadPool.QueueUserWorkItem(ListenerLoop);
        }


        public int ConnectedClientsCount
        {
            get { return _connectedClients.Count; }
        }

        public IEnumerable<TcpClient> ConnectedClients
        {
            get { return _connectedClients; }
        }

        internal bool QueueStop { get; set; }
        internal IPAddress IPAddress { get; }
        internal int Port { get; }
        internal int ReadLoopIntervalMs { get; set; }

        internal TcpListenerEx Listener { get; }


        private void StartThread()
        {
            if (_rxThread != null)
            {
                return;
            }

            _rxThread = new Thread(ListenerLoop);
            _rxThread.IsBackground = true;
            _rxThread.Start();
        }


        private void ListenerLoop(object state)
        {
            while (!QueueStop)
            {
                try
                {
                    RunLoopStep();
                }
                catch
                {
                }

                Thread.Sleep(ReadLoopIntervalMs);
            }

            Listener.Stop();
        }


        private bool IsSocketConnected(Socket s)
        {
            // https://stackoverflow.com/questions/2661764/how-to-check-if-a-socket-is-connected-disconnected-in-c
            var part1 = s.Poll(1000, SelectMode.SelectRead);
            var part2 = s.Available == 0;
            if ((part1 && part2) || !s.Connected)
            {
                return false;
            }

            return true;
        }


        private void RunLoopStep()
        {
            if (_disconnectedClients.Count > 0)
            {
                var disconnectedClients = _disconnectedClients.ToArray();
                _disconnectedClients.Clear();

                foreach (var disC in disconnectedClients)
                {
                    _connectedClients.Remove(disC);
                    _parent.NotifyClientDisconnected(this, disC);
                }
            }

            if (Listener.Pending())
            {
                var newClient = Listener.AcceptTcpClient();
                _connectedClients.Add(newClient);
                _parent.NotifyClientConnected(this, newClient);
            }

            _delimiter = _parent.Delimiter;

            foreach (var c in _connectedClients)
            {
                if (IsSocketConnected(c.Client) == false)
                {
                    _disconnectedClients.Add(c);
                }

                var bytesAvailable = c.Available;
                if (bytesAvailable == 0)
                {
                    //Thread.Sleep(10);
                    continue;
                }

                var bytesReceived = new List<byte>();

                while ((c.Available > 0) && c.Connected)
                {
                    var nextByte = new byte[1];
                    c.Client.Receive(nextByte, 0, 1, SocketFlags.None);
                    bytesReceived.AddRange(nextByte);

                    if (nextByte[0] == _delimiter)
                    {
                        var msg = _queuedMsg.ToArray();
                        _queuedMsg.Clear();
                        _parent.NotifyDelimiterMessageRx(this, c, msg);
                    }
                    else
                    {
                        _queuedMsg.AddRange(nextByte);
                    }
                }

                if (bytesReceived.Count > 0)
                {
                    _parent.NotifyEndTransmissionRx(this, c, bytesReceived.ToArray());
                }
            }
        }
    }
}