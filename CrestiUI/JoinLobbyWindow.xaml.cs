using System.Windows;

using CrestiUI.Game;

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


        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var userName = UserNameTextBox.Text;
            var ipToConnect = IpTextBox.Text;


            var lobby = new LocalLobby(ipToConnect, userName);
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
    }
}