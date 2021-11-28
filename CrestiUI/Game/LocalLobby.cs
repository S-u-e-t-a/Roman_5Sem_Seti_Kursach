using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.Json;

using CRESTI;

using CrestiUI.net;
using CrestiUI.Properties;

using Tcp;

namespace CrestiUI.Game
{
    public enum LobbyState
    {
        SearchingForPlayers,
        GameStarted
    }

    public class LocalLobby
    {
        public delegate void CellMarkedHandler(object sender, EventArgs e, int row, int col);

        public delegate void WritedToChatHandler(object sender, EventArgs e, string username, string message);

        public CellMarkedHandler CellMarked;
        public EventHandler PlayerUpdatedHandler;
        public EventHandler UsersUpdatedHandler;
        public EventHandler<int[]> GameStarted;
        public List<LocalUser> users;
        public string ChatHistory;
        public WritedToChatHandler WritedToChat;

        protected UserInLobby _user;


        public int PlayerCount
        {
            get { return users.Count; }
        }

        public LobbyState LobbyState { get; protected set; }
        public string Ip { get; }
        public string LobbyName { get; protected set; }

        public string LocalIps
        {
            get { return ""; }
        }

        public UserInLobby Oplayer { get; set; }


        public UserInLobby Xplayer { get; set; }


        public LocalLobby(string ipToConnect, string userName, string lobbyName)
        {
            LobbyName = lobbyName;
            _user = new UserInLobby(userName);
            users = new List<LocalUser>();
            users.Add(_user);
            var port = Settings.Default.DefaultPort;
            _user.tcpClient.DelimiterDataReceived += processMessage;
            _user.ConnectToLobby(ipToConnect, port);
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


        public LocalLobby(string ipToConnect, string userName, string lobbyName, LobbyState lobbyState, string chatHistory) : this(ipToConnect, userName, lobbyName)
        {
            ChatHistory = chatHistory;
            LobbyState = lobbyState;
        }


        public void MakeLocalUserPlayer(PlayerType playerType)
        {
            var request = new Request("POST", RequestCommands.POSTPlayerBecomePlayer, new Dictionary<string, string>
            {
                {"PlayerType", JsonSerializer.Serialize(playerType)},
                {"LocalUser", JsonSerializer.Serialize(_user)}
            });
            SendMessageToServer(request.ToJsonString());
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

            if (response.Name == "PlayerBecomePlayer")
            {
                var playerType = JsonSerializer.Deserialize<PlayerType>(response.Args["PlayerType"]);
                var player = JsonSerializer.Deserialize<UserInLobby>(response.Args["LocalUser"]);
                if (playerType == PlayerType.O)
                {
                    Oplayer = player;
                }

                if (playerType == PlayerType.X)
                {
                    Xplayer = player;
                }

                PlayerUpdatedHandler(this, null);
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