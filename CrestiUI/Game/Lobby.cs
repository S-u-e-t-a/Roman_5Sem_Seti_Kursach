using System.Collections.Generic;
using System.Linq;

using CRESTI;

using Tcp;

namespace CrestiUI.Game
{
    public class Lobby
    {
        private readonly SimpleTcpServer server;
        private Host lobbyHost;
        public UserInLobby OPlayer;
        private List<UserInLobby> users;

        public UserInLobby XPlayer;
        public string Name { get; }


        public Lobby(string name, Host host, int port)
        {
            server = new SimpleTcpServer();
            server.Start(port);
            Name = name;
            lobbyHost = host;
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