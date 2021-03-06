using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Text.Json;

using CrestiUI.net;

using Tcp;

namespace CrestiUI.Game
{
    public class ServerLobby : LocalLobby
    {
        private readonly SimpleTcpServer server;

        public override IEnumerable<string> LocalIps
        {
            get { return getIps(); }
        }


        public ServerLobby(string userName, string lobbyName, int port)
        {
            LobbyName = lobbyName;
            server = new SimpleTcpServer();
            server.Start(port, AddressFamily.InterNetwork);
            users = new List<LocalUser>();
            _user = new LocalUser(userName);

            server.DelimiterDataReceived += serverProcessMessage;

            LobbyState = LobbyState.SearchingForPlayers;
            tcpClient = new SimpleTcpClient();
            tcpClient.DelimiterDataReceived += processMessage;
            IsServer = true;
            connectToServerLobby("localhost", port);
            syncWithServer();
        }


        public IEnumerable<string> getIps()
        {
            return server.GetIPAddresses().Where(a => a.AddressFamily == AddressFamily.InterNetwork).Select(a => a.ToString());
        }


        public void StartGame()
        {
        }


        private void serverProcessMessage(object sender, Message message)
        {
            lock (this)
            {
                Trace.WriteLine($"На сервер пришло {message.MessageString}");
                var request = new Request(message.MessageString);
                if (request.RequestType == "GET")
                {
                    if (request.FuncName == RequestCommands.GETLobbyData.ToString())
                    {
                        var response = new Response("LobbyData", new Dictionary<string, string>
                        {
                            {"IsLobby", "true"},
                            {"State", LobbyState.ToString()},
                            {"Ip", JsonSerializer.Serialize(getIps())},
                            {"Name", LobbyName},
                            {"ChatHistory", ChatHistory}
                        });
                        message.ReplyLine(response.ToJsonString());
                    }

                    if (request.FuncName == RequestCommands.GETLobbyUsers.ToString())
                    {
                        var response = new Response("UserList", new Dictionary<string, string>
                        {
                            {"Users", JsonSerializer.Serialize(users)}
                        });
                        message.ReplyLine(response.ToJsonString());
                        //message.Reply(response.ToJsonString());
                    }
                }
                else if (request.RequestType == "POST")
                {
                    if (request.FuncName == RequestCommands.POSTUserJoinedLobby.ToString())
                    {
                        Trace.WriteLine("Чел зашел в лобби");
                        var userName = request.Args["UserName"];
                        users.Add(new LocalUser(userName));
                        UsersUpdatedHandler?.Invoke(this, null);
                        var response = new Response("UserList", new Dictionary<string, string>
                        {
                            {"Users", JsonSerializer.Serialize(users)}
                        });
                        server.BroadcastLine(response.ToJsonString());
                    }

                    if (request.FuncName == RequestCommands.POSTClientsMustUpdateUsers.ToString())
                    {
                        server.BroadcastLine(request.ToJsonString());
                    }

                    if (request.FuncName == RequestCommands.POSTGameStart.ToString())
                    {
                        var response = new Response("StartGame", null);
                        server.BroadcastLine(response.ToJsonString());
                        LobbyState = LobbyState.GameStarted;
                    }

                    if (request.FuncName == RequestCommands.POSTUserMark.ToString())
                    {
                        var col = request.Args["col"];
                        var row = request.Args["row"];
                        var response = new Response("MarkCell", new Dictionary<string, string>
                        {
                            {"row", row},
                            {"col", col}
                        });
                        server.BroadcastLine(response.ToJsonString());
                    }

                    if (request.FuncName == RequestCommands.POSTPlayerBecomePlayer.ToString())
                    {
                        var response = new Response("PlayerBecomePlayer", request.Args);
                        server.BroadcastLine(response.ToJsonString());
                    }

                    if (request.FuncName == RequestCommands.PostUserWriteToChat.ToString())
                    {
                        var response = new Response("UserWritedToChat", request.Args);
                        server.BroadcastLine(response.ToJsonString());
                    }

                    if (request.FuncName == RequestCommands.POSTRestartGame.ToString())
                    {
                        var response = new Response("RestartGame", null);
                        server.BroadcastLine(response.ToJsonString());
                    }
                }
            }
        }
    }
}