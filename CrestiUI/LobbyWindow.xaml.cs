using System.Windows;

using CrestiUI.Game;

namespace CrestiUI
{
    /// <summary>
    ///     Логика взаимодействия для LobbyWindow.xaml
    /// </summary>
    public partial class LobbyWindow : Window
    {
        public LobbyWindow(LocalLobby lobby)
        {
            DataContext = new LobbyWindowVM(lobby);
            InitializeComponent();
            getVM().LocalLobby.GameStarted += (sender, args) => gameStarted();
            StartButton.Visibility = Visibility.Hidden;
        }


        public LobbyWindow(ServerLobby lobby)
        {
            DataContext = new LobbyWindowVM(lobby);
            getVM().LocalLobby.GameStarted += (sender, args) => gameStarted();
            InitializeComponent();
        }


        private LobbyWindowVM getVM()
        {
            return DataContext as LobbyWindowVM;
        }


        private void gameStarted()
        {
            var gameWindow = new GameWindow(getVM().LocalLobby);
            gameWindow.Show();
            Close();
        }


        private void becomeX_Click(object sender, RoutedEventArgs e)
        {
        }


        private void becomeY_Click(object sender, RoutedEventArgs e)
        {
        }
    }
}