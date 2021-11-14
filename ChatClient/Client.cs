using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;

using CRESTI;

namespace ChatClient
{
    internal class Client
    {
        private const string host = "127.0.0.1";
        private const int port = 8888;
        private static string userName;
        private static TcpClient client;
        private static NetworkStream stream;
        private static Board _game;


        public Client(Board game)
        {
            _game = game;
            Console.Write("Введите свое имя: ");
            userName = Console.ReadLine();
            client = new TcpClient();
            try
            {
                client.Connect(host, port); //подключение клиента
                stream = client.GetStream(); // получаем поток

                var message = userName;
                var data = Encoding.Unicode.GetBytes(message);
                stream.Write(data, 0, data.Length);

                // запускаем новый поток для получения данных
                var receiveThread = new Thread(ReceiveMessage);
                receiveThread.Start(); //старт потока
                Console.WriteLine("Добро пожаловать, {0}", userName);
                while (true)
                {
                    Console.WriteLine("Введите сообщение: ");
                    message = Console.ReadLine();
                    SendMessage(message);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                Disconnect();
            }
        }


        // отправка сообщений
        private static void SendMessage(string message)
        {
            var data = Encoding.Unicode.GetBytes(message);
            stream.Write(data, 0, data.Length);
        }


        // получение сообщений
        private static void ReceiveMessage()
        {
            while (true)
            {
                try
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
                    Console.WriteLine(message); //вывод сообщения
                }
                catch
                {
                    Console.WriteLine("Подключение прервано!"); //соединение было прервано
                    Console.ReadLine();
                    Disconnect();
                }
            }
        }


        private static void Disconnect()
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