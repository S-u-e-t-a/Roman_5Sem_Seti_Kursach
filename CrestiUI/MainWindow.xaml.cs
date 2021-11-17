using System.Diagnostics;
using System.Windows;

using CRESTI;

using Tcp;

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

            var r = new Request("Mark?row=1&col=1");
            var p = new Board();
            //Trace.WriteLine("______________________________________________");
            //p.printCell();
            //Trace.WriteLine("==============================================");
            //RequestHandler.ExecuteRequest(p, r);
            //p.printCell();
            //Trace.WriteLine("______________________________________________");
        }


        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var server = new SimpleTcpServer().Start(8910);
            server.DelimiterDataReceived += (o, message) => { server.BroadcastLine(message.MessageString); };
            var user = new SimpleTcpClient().Connect("127.0.0.1", 8910);
            var gameWindow = new GameWindow(user);

            gameWindow.Show();
        }


        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            var user = new SimpleTcpClient().Connect("127.0.0.1", 8910);
            var gameWindow = new GameWindow(user);
            gameWindow.Show();
        }
    }
}