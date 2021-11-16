using CrestiUI.Properties;

using Tcp;

namespace CrestiUI.Game
{
    public class UserInLobby
    {
        private readonly SimpleTcpClient serverClient;
        public string Name;


        public UserInLobby(string name)
        {
            Name = name;
            serverClient = new SimpleTcpClient();
        }


        public void ConnectToLobby(Lobby lobby)
        {
            var port = Settings.Default.DefaultPort;
            serverClient.Connect(lobby.getIp(), port);
        }
    }
}