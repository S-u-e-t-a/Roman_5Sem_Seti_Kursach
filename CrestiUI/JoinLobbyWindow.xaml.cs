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
            var lobbyWindow = new LobbyWindow(lobby);
            lobbyWindow.Show();
            Close();
        }
    }
}