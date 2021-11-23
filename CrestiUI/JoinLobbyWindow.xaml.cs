using System.Windows;

using CrestiUI.Game;

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
            var lobby = new LocalLobby(lobbyToConnect, userName);
            var lobbyWindow = new LobbyWindow(lobby);
            lobbyWindow.Show();

            Close();
        }
    }
}