//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Net;
//using System.Net.Sockets;
//using System.Text;
//using System.Threading;

//namespace CrestiUI
//{
//    public class ServerObject
//    {
//        private static TcpListener tcpListener; // сервер для прослушивания
//        private readonly List<ClientObject> clients = new(); // все подключения
//        public event IConnector.ClientServerMessageHandler Notify;


//        protected internal void AddConnection(ClientObject clientObject)
//        {
//            clients.Add(clientObject);
//        }


//        protected internal void RemoveConnection(string id)
//        {
//            // получаем по id закрытое подключение
//            var client = clients.FirstOrDefault(c => c.Id == id);
//            // и удаляем его из списка подключений
//            if (client != null)
//            {
//                clients.Remove(client);
//            }
//        }


//        private void send(object sender, ClientServerMessageEventArgs e)
//        {
//            //Trace.WriteLine($"Получил сообщение из ClientObject {e.Message}");
//            Notify?.Invoke(sender, e);
//        }


//        // прослушивание входящих подключений
//        protected internal void Listen()
//        {
//            try
//            {
//                tcpListener = new TcpListener(IPAddress.Any, 8888);
//                tcpListener.Start();
//                //Trace.WriteLine("Сервер запущен. Ожидание подключений...");

//                while (true)
//                {
//                    var tcpClient = tcpListener.AcceptTcpClient();

//                    var clientObject = new ClientObject(tcpClient, this);
//                    clientObject.Notify += send;
//                    var clientThread = new Thread(clientObject.Process);
//                    clientThread.Start();
//                }
//            }
//            catch (Exception ex)
//            {
//                //Trace.WriteLine(ex.Message);
//                Disconnect();
//            }
//        }


//        // трансляция сообщения подключенным клиентам
//        protected internal void BroadcastMessage(string message, string id)
//        {
//            var data = Encoding.Unicode.GetBytes(message);
//            for (var i = 0; i < clients.Count; i++)
//            {
//                if (clients[i].Id != id) // если id клиента не равно id отправляющего
//                {
//                    clients[i].Stream.Write(data, 0, data.Length); //передача данных
//                }
//            }
//        }


//        // отключение всех клиентов
//        protected internal void Disconnect()
//        {
//            tcpListener.Stop(); //остановка сервера

//            for (var i = 0; i < clients.Count; i++)
//            {
//                clients[i].Close(); //отключение клиента
//            }

//            Environment.Exit(0); //завершение процесса
//        }
//    }
//}

