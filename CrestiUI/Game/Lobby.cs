using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using CRESTI;

using CrestiUI.net;

using Tcp;

namespace CrestiUI.Game
{
    public class Lobby
    {
        public enum LobbyState
        {
            SearchingForPlayers,
            GameStarted,
            
        }
        private readonly SimpleTcpServer server;
        private Host lobbyHost;
        public UserInLobby OPlayer;
        private List<UserInLobby> users;

        private LobbyState lobbyState;
        public UserInLobby XPlayer;
        public string LobbyName { get; }

        public int CountOfPlayers
        {
            get
            {
                return users.Count;
            }
        }


        public Lobby(string lobbyName, Host host, int port)
        {
            server = new SimpleTcpServer();
            server.Start(port);

            server.DelimiterDataReceived += (sender, message) =>
            {
                Trace.WriteLine(message.MessageString);
                var request = new Request(message.MessageString);
                
                if (request.FuncName == RequestCommands.GetLobbyData.ToString())
                {
                    var response = new Response(new Dictionary<string, string>()
                    {
                        {"IsLobby", "true"},
                        {"State",lobbyState.ToString()},
                        {"Ip",getIp()},
                        {"Name",LobbyName},
                        {"CountOfPlayers", CountOfPlayers.ToString()}
                    });
                    message.ReplyLine(response.ToJsonString());
                }
                
            };
            LobbyName = lobbyName;
            lobbyHost = host;
            lobbyState = LobbyState.SearchingForPlayers;
        }



        private void setPlayer(UserInLobby user, PlayerType playerType)
        {
            if (playerType == PlayerType.O)
            {
                OPlayer = user;
            }
            else
            {
                XPlayer = user;
            }
        }


        public void AddUser(UserInLobby user)
        {
            user.ConnectToLobby(this);
            users.Add(user);
        }


        public string getIp()
        {
            return server.GetIPAddresses().First().ToString();
        }


        public void StartGame()
        {
        }
    }
}