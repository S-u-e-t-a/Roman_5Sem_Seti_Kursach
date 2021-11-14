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
            var r = new Request("print?bool2=true&string3=jopa&int1=1");

            //RequestHandler.ExecuteRequest(p, r);
        }


        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var server = new Server();
            var user = new Client();
            var gameWindow = new GameWindow(user);

            gameWindow.Show();
        }


        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            var user = new Client();
            var gameWindow = new GameWindow(user);
            gameWindow.Show();
        }
    }
}