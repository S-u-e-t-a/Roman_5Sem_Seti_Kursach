﻿using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.Json;

using CrestiUI.net;

using Tcp;

namespace CrestiUI.Game
{
    public class ServerLobby : LocalLobby
    {
        private readonly SimpleTcpServer server;


        public ServerLobby(string lobbyName, Host host, int port)
        {
            LobbyName = lobbyName;
            users = new List<LocalUser>();
            users.Add(host);
            server = new SimpleTcpServer();
            server.Start(port);
            server.DelimiterDataReceived += processMessage;
            LobbyState = LobbyState.SearchingForPlayers;
        }


        private void processMessage(object sender, Message message)
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
                        {"Ip", getIp()},
                        {"Name", LobbyName},
                        {"CountOfPlayers", PlayerCount.ToString()}
                    });
                    message.Reply(response.ToJsonString());
                }

                if (request.FuncName == RequestCommands.GETLobbyUsers.ToString())
                {
                    var response = new Response("UserList", new Dictionary<string, string>
                    {
                        {"Users", JsonSerializer.Serialize(users)}
                    });
                    message.ReplyLine(response.ToJsonString());
                    message.Reply(response.ToJsonString());
                }
            }
            else if (request.RequestType == "POST")
            {
                if (request.FuncName == RequestCommands.POSTUserJoinedLobby.ToString())
                {
                    Trace.WriteLine("Чел зашел в лобби");
                    var userName = request.Args["UserName"];
                    var ip = request.Args["UserIp"];
                    users.Add(new LocalUser(userName, ip));
                    UsersUpdatedHandler(this, null);
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
            }
        }


        public string getIp()
        {
            return server.GetIPAddresses().First().ToString();
        }


        public void StartGame()
        {
        }
    }
}