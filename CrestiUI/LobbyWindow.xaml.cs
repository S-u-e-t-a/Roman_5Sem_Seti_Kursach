using System.Windows;

using CrestiUI.Game;

namespace CrestiUI
{
    /// <summary>
    ///     Логика взаимодействия для LobbyWindow.xaml
    /// </summary>
    public partial class LobbyWindow : Window
    {
        private LocalLobby _serverLobby;


        public LobbyWindow(LocalLobby serverLobby)
        {
            InitializeComponent();
            _serverLobby = serverLobby;
        }
    }
}