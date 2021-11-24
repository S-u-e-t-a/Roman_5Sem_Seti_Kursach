using System.Collections.Generic;
using System.Diagnostics;

using CrestiUI.net;

using Tcp;

namespace CrestiUI.Game
{
    public class UserInLobby : LocalUser
    {
        public SimpleTcpClient tcpClient;


        public UserInLobby(string name) : base(name)
        {
            tcpClient = new SimpleTcpClient();
        }


        //private void NotifyDelimiterDataReceived(object sender, Message message)
        //{
        //    DelimiterDataReceived?.Invoke(sender, message);
        //}


        //private void NotifyDataReceived(object sender, Message message)
        //{
        //    DataReceived?.Invoke(sender, message);
        //}


        //public event EventHandler<Message> DelimiterDataReceived;
        //public event EventHandler<Message> DataReceived;


        //public Message WriteLineAndGetReply(string data, TimeSpan timeout)
        //{
        //    return serverClient.WriteLineAndGetReply(data, timeout);
        //}


        //public void WriteLine(string data)
        //{
        //    serverClient.WriteLine(data);
        //}


        public void ConnectToLobby(string ip, int port)
        {
            tcpClient.Connect(ip, port);
            var userIp = tcpClient.TcpClient.Client.RemoteEndPoint.ToString();
            var request = new Request("POST", RequestCommands.POSTUserJoinedLobby, new Dictionary<string, string>
            {
                {"UserIp", userIp},
                {"UserName", Name}
            });
            Trace.WriteLine($"отправил из UserInLobby {request.ToJsonString()} ");
            tcpClient.WriteLine(request.ToJsonString());
        }
    }
}