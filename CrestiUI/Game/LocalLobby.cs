using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.Json;

using CRESTI;

using CrestiUI.net;

using Tcp;

namespace CrestiUI.Game
{
    public class LocalLobby : LobbyInLobbyList
    {
        private readonly UserInLobby _user;
        public UserInLobby OPlayer;
        public List<LocalUser> users;


        public EventHandler UsersUpdatedHandler;
        public UserInLobby XPlayer;

        public override int PlayerCount
        {
            get { return users.Count; }
        }


        public LocalLobby(UserInLobby user, string lobbyName)
        {
            _user = user;
            _user.DelimiterDataReceived += processMessage;
            LobbyName = lobbyName;
            getUsers();
        }


        protected LocalLobby()
        {
        }


        private void processMessage(object sender, Message message)
        {
            Trace.WriteLine($"Пришло в LocalLobby {message.MessageString} ");
            var request = new Request(message.MessageString);
            if (request.RequestType == "GET")
            {
                //if (request.FuncName == RequestCommands.GETLobbyData.ToString())
                //{
                //    Trace.WriteLine(RequestCommands.GETLobbyData.ToString());
                //    var response = new Response(new Dictionary<string, string>
                //    {
                //        {"IsLobby", "true"},
                //        {"State", LobbyState.ToString()},
                //        {"Ip", getIp()},
                //        {"Name", LobbyName},
                //        {"CountOfPlayers", PlayerCount.ToString()}
                //    });
                //    message.Reply(response.ToJsonString());
                //}

                //if (request.FuncName == RequestCommands.GETLobbyUsers.ToString())
                //{
                //    var response = new Response(new Dictionary<string, string>
                //    {
                //        {"Users", JsonSerializer.Serialize(users)}
                //    });
                //    message.Reply(response.ToJsonString());
                //}
            }
            else if (request.RequestType == "POST")
            {
                Trace.WriteLine("Пришел post запрос");
                if (request.FuncName == RequestCommands.POSTUserJoinedLobby.ToString())
                {
                    var userName = request.Args["UserName"];
                    var ip = request.Args["UserIp"];
                    users.Add(new LocalUser(userName, ip));
                    UsersUpdatedHandler(null, null);
                }

                if (request.FuncName == RequestCommands.POSTClientsMustUpdateUsers.ToString())
                {
                    getUsers();
                    UsersUpdatedHandler(null, null);
                }
            }
        }


        protected void getUsers()
        {
            var request = new Request("GET", RequestCommands.GETLobbyUsers, null).ToJsonString();
            var ans = _user.WriteLineAndGetReply(request, TimeSpan.FromSeconds(3));
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