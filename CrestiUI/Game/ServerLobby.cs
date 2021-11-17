using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.Json;

using CrestiUI.net;

using Tcp;

namespace CrestiUI.Game
{
    public class ServerLobby : LocalLobby
    {
        private readonly SimpleTcpServer server;


        public ServerLobby(string lobbyName, Host host, int port)
        {
            LobbyName = lobbyName;
            users = new List<LocalUser>();
            users.Add(host);
            server = new SimpleTcpServer();
            server.Start(port);
            server.DelimiterDataReceived += processMessage;
            LobbyState = LobbyState.SearchingForPlayers;
        }


        private void processMessage(object sender, Message message)
        {
            Trace.WriteLine(message.MessageString);
            var request = new Request(message.MessageString);
            Trace.WriteLine(request.RequestType == "GET");
            if (request.RequestType == "GET")
            {
                if (request.FuncName == RequestCommands.GetLobbyData.ToString())
                {
                    Trace.WriteLine(RequestCommands.GetLobbyData.ToString());
                    var response = new Response(new Dictionary<string, string>
                    {
                        {"IsLobby", "true"},
                        {"State", LobbyState.ToString()},
                        {"Ip", getIp()},
                        {"Name", LobbyName},
                        {"CountOfPlayers", PlayerCount.ToString()}
                    });
                    message.Reply(response.ToJsonString());
                }

                if (request.FuncName == RequestCommands.GetLobbyUsers.ToString())
                {
                    var response = new Response(new Dictionary<string, string>
                    {
                        {"Users", JsonSerializer.Serialize(users)}
                    });
                    message.Reply(response.ToJsonString());
                }
            }
            else if (request.RequestType == "POST")
            {
                if (request.FuncName == RequestCommands.UserJoinedLobby.ToString())
                {
                    var userName = request.Args["UserName"];
                    var ip = request.Args["UserIp"];
                    users.Add(new LocalUser(userName, ip));
                }
            }
        }


        //public void AddUser(UserInLobby user)
        //{
        //    user.ConnectToLobby(this);
        //    users.Add(user);
        //}


        public string getIp()
        {
            return server.GetIPAddresses().First().ToString();
        }


        public void StartGame()
        {
        }
    }
}