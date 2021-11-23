using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.Json;

using CrestiUI.net;

using Tcp;

namespace CrestiUI.Game
{
    public class LocalLobby : LobbyInLobbyList
    {
        private readonly UserInLobby _user;

        public List<LocalUser> users;


        public EventHandler UsersUpdatedHandler;

        public UserInLobby Xplayer { get; set; }

        public UserInLobby Yplayer { get; set; }


        public override int PlayerCount
        {
            get { return users.Count; }
        }


        public LocalLobby(UserInLobby user, string lobbyName)
        {
            users = new List<LocalUser>();
            _user = user;
            users.Add(_user);
            _user.serverClient.DelimiterDataReceived += processMessage;
            LobbyName = lobbyName;
        }


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
        }


        protected void getUsers()
        {
            var request = new Request("GET", RequestCommands.GETLobbyUsers, null).ToJsonString();
            var ans = _user.serverClient.WriteLineAndGetReply(request, TimeSpan.FromSeconds(10));
            var response = new Response(ans.MessageString);
            users = JsonSerializer.Deserialize<List<LocalUser>>(response.Args["Users"]);
        }
    }
}