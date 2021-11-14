using System.Net;
using System.Net.Sockets;

namespace Tcp
{
    internal class ConnectedClient
    {
        public IPAddress ServerIP { get; internal set; }
        public TcpClient Client { get; internal set; }
    }
}