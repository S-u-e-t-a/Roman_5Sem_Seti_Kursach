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
        protected UserInLobby _user;
        public EventHandler GameStarted;

        public List<LocalUser> users;


        public EventHandler UsersUpdatedHandler;

        public UserInLobby Xplayer { get; set; }

        public UserInLobby Yplayer { get; set; }


        public override int PlayerCount
        {
            get { return users.Count; }
        }


        public LocalLobby(LobbyInLobbyList lobbyToConnect, string userName)
        {
            _user = new UserInLobby(userName);
            users = new List<LocalUser>();
            users.Add(_user);
            var port = Settings.Default.DefaultPort;
            _user.ConnectToLobby(lobbyToConnect.Ip, port);
            _user.tcpClient.DelimiterDataReceived += processMessage;
        }


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


        private void processMessage(object sender, Message message)
        {
            Trace.WriteLine($"Пришло в LocalLobby {message.MessageString} ");
            var response = new Response(message.MessageString);

            if (response.Name == "UserList")
            {
                users = JsonSerializer.Deserialize<List<LocalUser>>(response.Args["Users"]);
                UsersUpdatedHandler(this, null);
            }

            if (response.Name == "StartGame")
            {
                GameStarted(this, null);
            }
        }


        public void SendToServerGameStart()
        {
            var request = new Request("POST", RequestCommands.POSTGameStart, null);
            _user.tcpClient.WriteLine(request.ToJsonString());
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