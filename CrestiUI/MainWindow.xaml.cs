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
            //Trace.WriteLine("--------------------");
            //foreach (var ipAddress in server.GetIPAddresses())
            //{
            //    Trace.WriteLine(ipAddress);
            //}

            //Trace.WriteLine("--------------------");
            //foreach (var ipAddress in server.GetListeningIPs())
            //{
            //    Trace.WriteLine(ipAddress);
            //}

            //Trace.WriteLine("--------------------");
            var user = new SimpleTcpClient().Connect("127.0.0.1", 8910);
            server.ClientConnected += (sender, message) => { Trace.WriteLine("------------подключился чел---------------"); };
            //foreach (var ipAddress in IPGlobalProperties.GetIPGlobalProperties()
            //    .GetActiveTcpListeners())
            //{
            //    if (ipAddress.Port == 8910)
            //    {
            //        Trace.WriteLine($"{ipAddress} {ipAddress.AddressFamily} ");
            //    }
            //}

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