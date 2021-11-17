using System.Collections.Generic;
using System.Text.Json;
using System.Windows;

using CRESTI;

using CrestiUI.Game;

namespace CrestiUI
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly Board game;


        public MainWindow()
        {
            InitializeComponent();
            var s =
                "[{\u0022Name\u0022:\u0022\\u0418\\u043C\\u044F \\u043F\\u043E\\u043B\\u044C\\u0437\\u043E\\u0432\\u0430\\u0442\\u0435\\u043B\\u044F\u0022,\u0022Ip\u0022:null}]";
            var users = JsonSerializer.Deserialize<List<LocalUser>>(s);
        }


        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var lobbyListWindow = new LobbyListWindow();
            lobbyListWindow.Show();
            Close();
        }
    }
}