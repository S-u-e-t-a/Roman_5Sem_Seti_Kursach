using System.Windows;

using CRESTI;

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
        }


        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var lobbyListWindow = new LobbyListWindow();
            lobbyListWindow.Show();
            Close();
        }
    }
}