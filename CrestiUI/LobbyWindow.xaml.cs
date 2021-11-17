using System;
using System.Windows;

using CrestiUI.Game;

namespace CrestiUI
{
    /// <summary>
    ///     Логика взаимодействия для LobbyWindow.xaml
    /// </summary>
    public partial class LobbyWindow : Window
    {
        private readonly LocalLobby _serverLobby;


        public LobbyWindow(LocalLobby serverLobby)
        {
            InitializeComponent();
            _serverLobby = serverLobby;
            updateUserGrid(null, null);
            _serverLobby.UsersUpdatedHandler += (sender, args) => { updateUserGrid(sender, args); };
        }


        private void updateUserGrid(object sender, EventArgs e)
        {
            userGrid.ItemsSource = _serverLobby.users;
        }
    }
}