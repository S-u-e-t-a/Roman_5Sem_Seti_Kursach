using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace CrestiUI
{
    public class Client : IConnector
    {
        private const string host = "127.0.0.1";
        private const int port = 8888;
        private static TcpClient client;
        private static NetworkStream stream;
        private static string userName;


        public Client()
        {
            client = new TcpClient();

            client.Connect(host, port); //подключение клиента
            stream = client.GetStream(); // получаем поток
            // запускаем новый поток для получения данных
            var receiveThread = new Thread(ReceiveMessage);
            receiveThread.Start(); //старт потока
            //while (true)
            //{
            //    Console.WriteLine("Введите сообщение: ");
            //    message = "Console.ReadLine()";
            //    SendMessage(message);
            //}
        }


        public event IConnector.ClientServerMessageHandler Notify;


        // отправка сообщений
        public void SendMessage(string message)
        {
            var data = Encoding.Unicode.GetBytes(message);
            stream.Write(data, 0, data.Length);
        }


        // получение сообщений
        private void ReceiveMessage()
        {
            while (true)
            {
                var data = new byte[64]; // буфер для получаемых данных
                var builder = new StringBuilder();
                var bytes = 0;
                do
                {
                    bytes = stream.Read(data, 0, data.Length);
                    builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
                } while (stream.DataAvailable);

                var message = builder.ToString();
                Notify?.Invoke(this, new ClientServerMessageEventArgs(message));
            }
        }


        public void Disconnect()
        {
            if (stream != null)
            {
                stream.Close(); //отключение потока
            }

            if (client != null)
            {
                client.Close(); //отключение клиента
            }

            Environment.Exit(0); //завершение процесса
        }
    }
}