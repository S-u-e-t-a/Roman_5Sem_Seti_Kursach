using System;
using System.Threading;

namespace CrestiUI
{
    internal class Server : IConnector
    {
        private static ServerObject server; // сервер
        private static Thread listenThread; // поток для прослушивания


        public Server()
        {
            try
            {
                server = new ServerObject();
                server.Notify += send;
                listenThread = new Thread(server.Listen);
                listenThread.Start(); //старт потока
            }
            catch (Exception ex)
            {
                server.Disconnect();
                //Trace.WriteLine(ex.Message);
            }
        }


        public event IConnector.ClientServerMessageHandler Notify;


        private void send(object sender, ClientServerMessageEventArgs e)
        {
            //Trace.WriteLine($"Получил сообщение из ServerObject {e.Message}");
            Notify?.Invoke(sender, e);
        }
    }
}