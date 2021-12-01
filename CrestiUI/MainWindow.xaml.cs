using System.Windows;

using CRESTI;

namespace CrestiUI
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }


        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var joinLobbyWindow = new JoinLobbyWindow();
            joinLobbyWindow.Show();
            Close();
        }


        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            var createLobbyWindow = new CreateLobbyWindow();
            createLobbyWindow.Show();
            Close();
        }
    }
}