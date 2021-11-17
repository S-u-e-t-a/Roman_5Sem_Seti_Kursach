using System;
using System.Collections.Generic;

using CrestiUI.net;

using Tcp;

namespace CrestiUI.Game
{
    public class UserInLobby : LocalUser
    {
        private SimpleTcpClient serverClient;


        public UserInLobby(string name) : base(name)
        {
        }


        public Message WriteLineAndGetReply(string data, TimeSpan timeout)
        {
            return serverClient.WriteLineAndGetReply(data, timeout);
        }


        public void WriteLine(string data)
        {
            serverClient.WriteLine(data);
        }


        public void ConnectToLobby(string ip, int port)
        {
            serverClient = new SimpleTcpClient();
            serverClient.Connect(ip, port);
            var userIp = serverClient.TcpClient.Client.RemoteEndPoint.ToString();
            var request = new Request("POST", RequestCommands.UserJoinedLobby, new Dictionary<string, string>
            {
                {"UserIp", userIp},
                {"UserName", Name}
            });
        }
    }
}