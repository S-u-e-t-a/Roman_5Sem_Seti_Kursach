using System.Windows;

using CrestiUI.Game;

namespace CrestiUI
{
    /// <summary>
    ///     Логика взаимодействия для LobbyWindow.xaml
    /// </summary>
    public partial class LobbyWindow : Window
    {
        private readonly LocalLobby _lobby;


        public LobbyWindow(LocalLobby lobby)
        {
            _lobby = lobby;
            DataContext = new LobbyWindowVM(_lobby);
            _lobby.GameStarted += (sender, args) => gameStarted();
            InitializeComponent();
            StartButton.Visibility = Visibility.Hidden;
        }


        public LobbyWindow(ServerLobby lobby)
        {
            _lobby = lobby;
            DataContext = new LobbyWindowVM(_lobby);
            _lobby.GameStarted += (sender, args) => gameStarted();
            InitializeComponent();
        }


        private void gameStarted()
        {
            Dispatcher.Invoke(() =>
            {
                var gameWindow = new GameWindow(_lobby);
                gameWindow.Show();
                Close();
            });
        }
    }
}