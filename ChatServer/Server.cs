using System;
using System.Threading;

using ChatClient;

using CRESTI;

namespace ChatServer
{
    internal class Server
    {
        public delegate void ClientServerMessageHandler(object sender, ClientServerMessageEventArgs e);

        private static ServerObject server; // сервер
        private static Thread listenThread; // потока для прослушивания
        private static Board _game;


        public Server(Board game)
        {
            _game = game;
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
                Console.WriteLine(ex.Message);
            }
        }


        public event ClientServerMessageHandler Notify;


        private void send(object sender, ClientServerMessageEventArgs e)
        {
            Console.WriteLine($"Получил сообщение из ServerObject {e.Message}");
            RequestHandler.ExecuteRequest(_game, new Request(e.Message));
        }


        public void MarkCell(int row, int col)
        {
        }
    }
}