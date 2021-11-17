using System.Windows;

using CrestiUI.Game;
using CrestiUI.Properties;

namespace CrestiUI
{
    /// <summary>
    ///     Логика взаимодействия для CreateLobbyWindow.xaml
    /// </summary>
    public partial class CreateLobbyWindow : Window
    {
        public CreateLobbyWindow()
        {
            InitializeComponent();
        }


        private void CreateLobby_Click(object sender, RoutedEventArgs e)
        {
            var lobbyName = LobbyNameTextBox.Text;
            var userName = UserNameTextBox.Text;
            var port = Settings.Default.DefaultPort;
            var host = new Host(userName);
            var lobby = new ServerLobby(lobbyName, host, port);
            var lobbyWindow = new LobbyWindow(lobby);

            lobbyWindow.Show();
            Close();
        }
    }
}