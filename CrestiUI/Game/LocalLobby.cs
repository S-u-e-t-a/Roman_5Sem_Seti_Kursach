using System;
using System.Collections.Generic;
using System.Text.Json;

using CRESTI;

using CrestiUI.net;

namespace CrestiUI.Game
{
    public class LocalLobby : LobbyInLobbyList
    {
        private readonly UserInLobby _user;
        public UserInLobby OPlayer;
        protected List<LocalUser> users;
        public UserInLobby XPlayer;

        public override int PlayerCount
        {
            get { return users.Count; }
        }


        public LocalLobby(UserInLobby user, string lobbyName)
        {
            _user = user;
            LobbyName = lobbyName;
            getUsers();
        }


        protected LocalLobby()
        {
        }


        protected void getUsers()
        {
            var request = new Request("GET", RequestCommands.GetLobbyUsers, null).ToJsonString();
            var ans = _user.WriteLineAndGetReply(request, TimeSpan.Zero);
            var response = new Response(ans.MessageString);
            users = JsonSerializer.Deserialize<List<LocalUser>>(response.ResponseArgs["Users"]);
        }


        private void setPlayer(UserInLobby user, PlayerType playerType)
        {
            if (playerType == PlayerType.O)
            {
                OPlayer = user;
            }
            else
            {
                XPlayer = user;
            }
        }
    }
}