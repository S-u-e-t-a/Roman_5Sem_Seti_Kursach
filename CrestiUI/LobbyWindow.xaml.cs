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
            DataContext = new LobbyWindowVM(lobby);
            InitializeComponent();
        }


        private void becomeX_Click(object sender, RoutedEventArgs e)
        {
            // _lobby.setPlayer(_lobby.u);
        }


        private void becomeY_Click(object sender, RoutedEventArgs e)
        {
        }
    }
}