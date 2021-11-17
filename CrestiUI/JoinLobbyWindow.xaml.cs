using System.Windows;

using CrestiUI.Game;
using CrestiUI.Properties;

namespace CrestiUI
{
    /// <summary>
    ///     Логика взаимодействия для JoinLobbyWindow.xaml
    /// </summary>
    public partial class JoinLobbyWindow : Window
    {
        private readonly LobbyInLobbyList lobbyToConnect;


        public JoinLobbyWindow(LobbyInLobbyList lobby)
        {
            InitializeComponent();
            lobbyToConnect = lobby;
        }


        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var userName = UserNameTextBox.Text;
            var port = Settings.Default.DefaultPort;
            var user = new UserInLobby(userName);
            user.ConnectToLobby(lobbyToConnect.Ip, port);
            var lobby = new LocalLobby(user, lobbyToConnect.LobbyName);


            var lobbyWindow = new LobbyWindow(lobby);
            lobbyWindow.Show();

            Close();
        }
    }
}