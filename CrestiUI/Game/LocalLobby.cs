using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.Json;

using CrestiUI.net;
using CrestiUI.Properties;

using Tcp;

namespace CrestiUI.Game
{
    public class LocalLobby : LobbyInLobbyList
    {
        public delegate void CellMarkedHandler(object sender, EventArgs e, int row, int col);

        public delegate void WritedToChatHandler(object sender, EventArgs e, string username, string message);

        public CellMarkedHandler CellMarked;
        public EventHandler UsersUpdatedHandler;
        public EventHandler<int[]> GameStarted;

        public List<LocalUser> users;

        public WritedToChatHandler WritedToChat;

        protected UserInLobby _user;


        public override int PlayerCount
        {
            get { return users.Count; }
        }


        public UserInLobby Xplayer { get; set; }

        public UserInLobby Yplayer { get; set; }


        public LocalLobby(LobbyInLobbyList lobbyToConnect, string userName)
        {
            _user = new UserInLobby(userName);
            users = new List<LocalUser>();
            users.Add(_user);
            var port = Settings.Default.DefaultPort;
            _user.tcpClient.DelimiterDataReceived += processMessage;
            _user.ConnectToLobby(lobbyToConnect.Ip, port);
        }
        //public void connectToLobby()


        //public LocalLobby(UserInLobby user, string lobbyName)
        //{
        //    users = new List<LocalUser>();
        //    _user = user;
        //    users.Add(_user);
        //    _user.serverClient.DelimiterDataReceived += processMessage;
        //    LobbyName = lobbyName;
        //}


        protected LocalLobby()
        {
        }


        protected void processMessage(object sender, Message message)
        {
            Trace.WriteLine($"Пришло в LocalLobby {message.MessageString} ");
            var response = new Response(message.MessageString);

            if (response.Name == "UserList")
            {
                users = JsonSerializer.Deserialize<List<LocalUser>>(response.Args["Users"]);
                UsersUpdatedHandler?.Invoke(this, null);
            }

            if (response.Name == "StartGame")
            {
                GameStarted(this, null);
            }

            if (response.Name == "MarkCell")
            {
                var col = Convert.ToInt32(response.Args["col"]);
                var row = Convert.ToInt32(response.Args["row"]);
                CellMarked(this, null, row, col);
            }

            if (response.Name == "WriteToChat")
            {
                var userName = response.Args["UserName"];
                var mes = response.Args["Message"];
                WritedToChat(this, null, userName, mes);
            }
        }


        public void SendToServerGameStart()
        {
            var request = new Request("POST", RequestCommands.POSTGameStart, null);
            _user.tcpClient.WriteLine(request.ToJsonString());
        }


        public void SendMessageToServer(string message)
        {
            _user.tcpClient.WriteLine(message);
        }


        protected void getUsers()
        {
            var request = new Request("GET", RequestCommands.GETLobbyUsers, null).ToJsonString();
            var ans = _user.tcpClient.WriteLineAndGetReply(request, TimeSpan.FromSeconds(10));
            var response = new Response(ans.MessageString);
            users = JsonSerializer.Deserialize<List<LocalUser>>(response.Args["Users"]);
        }
    }
}