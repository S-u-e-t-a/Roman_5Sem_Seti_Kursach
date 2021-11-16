using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Tcp
{
    public class SimpleTcpServer
    {
        private readonly List<ServerListener> _listeners = new();


        public byte Delimiter { get; set; }
        public Encoding StringEncoder { get; set; }
        public bool AutoTrimStrings { get; set; }

        public bool IsStarted
        {
            get { return _listeners.Any(l => l.Listener.Active); }
        }

        public int ConnectedClientsCount
        {
            get { return _listeners.Sum(l => l.ConnectedClientsCount); }
        }


        public SimpleTcpServer()
        {
            Delimiter = 0x13;
            StringEncoder = Encoding.UTF8;
        }


        public event EventHandler<TcpClient> ClientConnected;
        public event EventHandler<TcpClient> ClientDisconnected;
        public event EventHandler<Message> DelimiterDataReceived;
        public event EventHandler<Message> DataReceived;


        public IEnumerable<IPAddress> GetIPAddresses()
        {
            var ipAddresses = new List<IPAddress>();

            var enabledNetInterfaces = NetworkInterface.GetAllNetworkInterfaces()
                .Where(nic => nic.OperationalStatus == OperationalStatus.Up);
            foreach (var netInterface in enabledNetInterfaces)
            {
                var ipProps = netInterface.GetIPProperties();
                foreach (var addr in ipProps.UnicastAddresses)
                {
                    if (!ipAddresses.Contains(addr.Address))
                    {
                        ipAddresses.Add(addr.Address);
                    }
                }
            }

            var ipSorted = ipAddresses.OrderByDescending(ip => RankIpAddress(ip)).ToList();

            return ipSorted;
        }


        public List<IPAddress> GetListeningIPs()
        {
            var listenIps = new List<IPAddress>();
            foreach (var l in _listeners)
            {
                if (!listenIps.Contains(l.IPAddress))
                {
                    listenIps.Add(l.IPAddress);
                }
            }

            return listenIps.OrderByDescending(ip => RankIpAddress(ip)).ToList();
        }


        public void Broadcast(byte[] data)
        {
            foreach (var client in _listeners.SelectMany(x => x.ConnectedClients))
            {
                client.GetStream().Write(data, 0, data.Length);
            }
        }


        public void Broadcast(string data)
        {
            if (data == null)
            {
                return;
            }

            Broadcast(StringEncoder.GetBytes(data));
        }


        public void BroadcastLine(string data)
        {
            if (string.IsNullOrEmpty(data))
            {
                return;
            }

            if (data.LastOrDefault() != Delimiter)
            {
                Broadcast(data + StringEncoder.GetString(new[] {Delimiter}));
            }
            else
            {
                Broadcast(data);
            }
        }


        private int RankIpAddress(IPAddress addr)
        {
            var rankScore = 1000;

            if (IPAddress.IsLoopback(addr))
            {
                // rank loopback below others, even though their routing metrics may be better
                rankScore = 300;
            }
            else if (addr.AddressFamily == AddressFamily.InterNetwork)
            {
                rankScore += 100;
                // except...
                if (addr.GetAddressBytes().Take(2).SequenceEqual(new byte[] {169, 254}))
                {
                    // APIPA generated address - no router or DHCP server - to the bottom of the pile
                    rankScore = 0;
                }
            }

            if (rankScore > 500)
            {
                foreach (var nic in TryGetCurrentNetworkInterfaces())
                {
                    var ipProps = nic.GetIPProperties();
                    if (ipProps.GatewayAddresses.Any())
                    {
                        if (ipProps.UnicastAddresses.Any(u => u.Address.Equals(addr)))
                        {
                            // if the preferred NIC has multiple addresses, boost all equally
                            // (justifies not bothering to differentiate... IOW YAGNI)
                            rankScore += 1000;
                        }

                        // only considering the first NIC that is UP and has a gateway defined
                        break;
                    }
                }
            }

            return rankScore;
        }


        private static IEnumerable<NetworkInterface> TryGetCurrentNetworkInterfaces()
        {
            try
            {
                return NetworkInterface.GetAllNetworkInterfaces().Where(ni => ni.OperationalStatus == OperationalStatus.Up);
            }
            catch (NetworkInformationException)
            {
                return Enumerable.Empty<NetworkInterface>();
            }
        }


        public SimpleTcpServer Start(int port, bool ignoreNicsWithOccupiedPorts = true)
        {
            var ipSorted = GetIPAddresses();
            var anyNicFailed = false;
            foreach (var ipAddr in ipSorted)
            {
                try
                {
                    Start(ipAddr, port);
                }
                catch (SocketException ex)
                {
                    DebugInfo(ex.ToString());
                    anyNicFailed = true;
                }
            }

            if (!IsStarted)
            {
                throw new InvalidOperationException("Port was already occupied for all network interfaces");
            }

            if (anyNicFailed && !ignoreNicsWithOccupiedPorts)
            {
                Stop();

                throw new InvalidOperationException("Port was already occupied for one or more network interfaces.");
            }

            return this;
        }


        public SimpleTcpServer Start(int port, AddressFamily addressFamilyFilter)
        {
            var ipSorted = GetIPAddresses().Where(ip => ip.AddressFamily == addressFamilyFilter);
            foreach (var ipAddr in ipSorted)
            {
                try
                {
                    Start(ipAddr, port);
                }
                catch
                {
                }
            }

            return this;
        }


        public SimpleTcpServer Start(IPAddress ipAddress, int port)
        {
            var listener = new ServerListener(this, ipAddress, port);
            _listeners.Add(listener);

            return this;
        }


        public void Stop()
        {
            _listeners.All(l => l.QueueStop = true);
            while (_listeners.Any(l => l.Listener.Active))
            {
                Thread.Sleep(100);
            }

            ;
            _listeners.Clear();
        }


        internal void NotifyDelimiterMessageRx(ServerListener listener, TcpClient client, byte[] msg)
        {
            if (DelimiterDataReceived != null)
            {
                var m = new Message(msg, client, StringEncoder, Delimiter, AutoTrimStrings);
                DelimiterDataReceived(this, m);
            }
        }


        internal void NotifyEndTransmissionRx(ServerListener listener, TcpClient client, byte[] msg)
        {
            if (DataReceived != null)
            {
                var m = new Message(msg, client, StringEncoder, Delimiter, AutoTrimStrings);
                DataReceived(this, m);
            }
        }


        internal void NotifyClientConnected(ServerListener listener, TcpClient newClient)
        {
            if (ClientConnected != null)
            {
                ClientConnected(this, newClient);
            }
        }


        internal void NotifyClientDisconnected(ServerListener listener, TcpClient disconnectedClient)
        {
            if (ClientDisconnected != null)
            {
                ClientDisconnected(this, disconnectedClient);
            }
        }


        #region Debug logging

        [Conditional("DEBUG")]
        private void DebugInfo(string format, params object[] args)
        {
            if (_debugInfoTime == null)
            {
                _debugInfoTime = new Stopwatch();
                _debugInfoTime.Start();
            }

            Debug.WriteLine(_debugInfoTime.ElapsedMilliseconds + ": " + format, args);
        }


        private Stopwatch _debugInfoTime;

        #endregion Debug logging
    }
}