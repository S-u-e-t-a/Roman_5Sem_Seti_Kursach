using System;
using System.Net.Sockets;
using System.Text;

namespace ChatServer
{
    public class ClientObject
    {
        public delegate void ClientServerMessageHandler(object sender, ClientServerMessageEventArgs e);

        private readonly TcpClient client;
        private readonly ServerObject server; // объект сервера
        private string userName;


        public ClientObject(TcpClient tcpClient, ServerObject serverObject)
        {
            Id = Guid.NewGuid().ToString();
            client = tcpClient;
            server = serverObject;
            serverObject.AddConnection(this);
        }


        protected internal string Id { get; }
        protected internal NetworkStream Stream { get; private set; }
        public event ClientServerMessageHandler Notify;


        public void Process()
        {
            try
            {
                Stream = client.GetStream();
                // получаем имя пользователя
                var message = GetMessage();
                userName = message;

                message = userName + " вошел в чат";
                // посылаем сообщение о входе в чат всем подключенным пользователям
                server.BroadcastMessage(message, Id);
                Console.WriteLine(message);
                // в бесконечном цикле получаем сообщения от клиента
                while (true)
                {
                    try
                    {
                        message = GetMessage();

                        //message = string.Format("{0}: {1}", userName, message);
                        //Console.WriteLine(message);
                        Notify?.Invoke(this, new ClientServerMessageEventArgs(message));
                        server.BroadcastMessage(message, Id);
                    }
                    catch (NullReferenceException)
                    {
                        message = string.Format("{0}: покинул чат", userName);
                        Console.WriteLine(message);
                        server.BroadcastMessage(message, Id);

                        break;
                    }
                }
            }
            catch (NullReferenceException e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                // в случае выхода из цикла закрываем ресурсы
                server.RemoveConnection(Id);
                Close();
            }
        }


        // чтение входящего сообщения и преобразование в строку
        private string GetMessage()
        {
            var data = new byte[64]; // буфер для получаемых данных
            var builder = new StringBuilder();
            var bytes = 0;
            do
            {
                bytes = Stream.Read(data, 0, data.Length);
                builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
            } while (Stream.DataAvailable);

            return builder.ToString();
        }


        // закрытие подключения
        protected internal void Close()
        {
            if (Stream != null)
            {
                Stream.Close();
            }

            if (client != null)
            {
                client.Close();
            }
        }
    }
}