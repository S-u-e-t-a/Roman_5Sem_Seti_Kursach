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

        //public delegate void GameFinishedHandler(object sender, string winnerName);

        public delegate void WritedToChatHandler(object sender, EventArgs e, string username, string message);

        public CellMarkedHandler CellMarked;
        public EventHandler GameRestartedHandler;
        public EventHandler PlayerUpdatedHandler;
        public EventHandler UsersUpdatedHandler;
        public EventHandler<int[]> GameStarted;

        //public GameFinishedHandler GameFinished;
        public List<LocalUser> users;
        public string ChatHistory;
        public WritedToChatHandler WritedToChat;

        protected LocalUser _user;
        protected SimpleTcpClient tcpClient;

        public bool IsServer { get; protected set; }

        public virtual IEnumerable<string> LocalIps { get; private set; }

        public int PlayerCount
        {
            get { return users.Count; }
        }

        public LobbyState LobbyState { get; protected set; }
        public LocalUser Oplayer { get; set; }


        public LocalUser Xplayer { get; set; }
        public string LobbyName { get; protected set; }


        public LocalLobby(string ipToConnect, string userName)
        {
            _user = new LocalUser(userName);
            users = new List<LocalUser>();
            users.Add(_user);
            tcpClient = new SimpleTcpClient();
            tcpClient.DelimiterDataReceived += processMessage;

            var port = Settings.Default.DefaultPort;

            connectToServerLobby(ipToConnect, port);
            syncWithServer();
        }


        protected LocalLobby()
        {
        }


        public bool isPlayerUser()
        {
            return isOPlayer() || isXPlayer();
        }


        public bool isXPlayer()
        {
            return _user.Name == Xplayer.Name;
        }


        public bool isOPlayer()
        {
            return _user.Name == Oplayer.Name;
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


        public void SendToServerGameStart()
        {
            LobbyState = LobbyState.GameStarted;
            var request = new Request("POST", RequestCommands.POSTGameStart, null);
            tcpClient.WriteLine(request.ToJsonString());
        }


        public void WriteToChat(string message)
        {
            var request = new Request("POST", RequestCommands.PostUserWriteToChat, new Dictionary<string, string>
            {
                {"UserName", _user.Name},
                {"Message", message}
            });
            tcpClient.WriteLine(request.ToJsonString());
        }


        public void SendMessageToServer(string message)
        {
            tcpClient.WriteLine(message);
        }


        protected void connectToServerLobby(string ip, int port)
        {
            tcpClient.Connect(ip, port);
            var request = new Request("POST", RequestCommands.POSTUserJoinedLobby, new Dictionary<string, string>
            {
                {"UserName", _user.Name}
            });
            tcpClient.WriteLine(request.ToJsonString());
        }


        //public LocalLobby(string ipToConnect, string userName, string lobbyName, LobbyState lobbyState, string chatHistory) : this(ipToConnect, userName, lobbyName)
        //{
        //    ChatHistory = chatHistory;
        //    LobbyState = lobbyState;
        //}


        protected void syncWithServer()
        {
            lock (this)
            {
                var ans = tcpClient.WriteLineAndGetReply(new Request("GET", RequestCommands.GETLobbyData, null).ToJsonString(), TimeSpan.FromSeconds(3));
                if (ans != null)
                {
                    var lobbyData = new Response(ans.MessageString);
                    if (lobbyData.Args["IsLobby"] == "true")
                    {
                        LobbyName = lobbyData.Args["Name"];
                        LobbyState lobbyState;
                        Enum.TryParse(lobbyData.Args["State"], out lobbyState);
                        LobbyState = lobbyState;
                        ChatHistory = lobbyData.Args["ChatHistory"];
                        LocalIps = JsonSerializer.Deserialize<IEnumerable<string>>(lobbyData.Args["Ip"]);
                    }
                    else
                    {
                        throw new ArgumentException();
                    }
                }
            }
        }


        protected void processMessage(object sender, Message message)
        {
            lock (this)
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

                if (response.Name == "UserWritedToChat")
                {
                    var userName = response.Args["UserName"];
                    var mes = response.Args["Message"];
                    ChatHistory += $"{userName}: {mes} \n";
                    WritedToChat(this, null, userName, mes);
                }

                if (response.Name == "PlayerBecomePlayer")
                {
                    var playerType = JsonSerializer.Deserialize<PlayerType>(response.Args["PlayerType"]);
                    var player = JsonSerializer.Deserialize<LocalUser>(response.Args["LocalUser"]);
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

                if (response.Name == "RestartGame")
                {
                    GameRestartedHandler(this, null);
                }
            }
        }


        protected void getUsers()
        {
            var request = new Request("GET", RequestCommands.GETLobbyUsers, null).ToJsonString();
            var ans = tcpClient.WriteLineAndGetReply(request, TimeSpan.FromSeconds(10));
            var response = new Response(ans.MessageString);
            users = JsonSerializer.Deserialize<List<LocalUser>>(response.Args["Users"]);
        }
    }
}