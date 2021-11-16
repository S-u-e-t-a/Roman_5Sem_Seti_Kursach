using System.Windows;

using CrestiUI.Game;

namespace CrestiUI
{
    /// <summary>
    ///     Логика взаимодействия для LobbyWindow.xaml
    /// </summary>
    public partial class LobbyWindow : Window
    {
        private Lobby lobby;


        public LobbyWindow(Lobby lobby)
        {
            InitializeComponent();
            this.lobby = lobby;
        }
    }
}