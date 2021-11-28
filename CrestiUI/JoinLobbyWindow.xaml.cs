using System;
using System.Windows;

using CrestiUI.Game;
using CrestiUI.net;
using CrestiUI.Properties;

using Tcp;

namespace CrestiUI
{
    /// <summary>
    ///     Логика взаимодействия для JoinLobbyWindow.xaml
    /// </summary>
    public partial class JoinLobbyWindow : Window
    {
        public JoinLobbyWindow()
        {
            InitializeComponent();
        }


        private Response getLobbyData(string ip)
        {
            var asker = new SimpleTcpClient();
            var port = Settings.Default.DefaultPort;
            asker.Connect(ip, port);
            var ans = asker.WriteLineAndGetReply(new Request("GET", RequestCommands.GETLobbyData, null).ToJsonString(), TimeSpan.FromSeconds(3));
            if (ans != null)
            {
                var response = new Response(ans.MessageString);
                if (response.Args["IsLobby"] == "true")
                {
                    return response;
                }

                asker.Disconnect();
            }

            return null;
        }


        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var userName = UserNameTextBox.Text;
            var ipToConnect = IpTextBox.Text;
            var lobbyData = getLobbyData(ipToConnect);
            if (lobbyData != null)
            {
                var lobbyName = lobbyData.Args["Name"];
                LobbyState lobbyState;
                Enum.TryParse(lobbyData.Args["State"], out lobbyState);
                var chatHistory = lobbyData.Args["ChatHistory"];
                var lobby = new LocalLobby(ipToConnect, userName, lobbyName, lobbyState, chatHistory);
                if (lobby.LobbyState == LobbyState.SearchingForPlayers)
                {
                    var lobbyWindow = new LobbyWindow(lobby);
                    lobbyWindow.Show();
                }
                else
                {
                    var gameWindow = new GameWindow(lobby);
                    gameWindow.Show();
                }

                Close();
            }
            else
            {
                MessageBox.Show("Не удалось подключиться к серверу");
            }
        }
    }
}