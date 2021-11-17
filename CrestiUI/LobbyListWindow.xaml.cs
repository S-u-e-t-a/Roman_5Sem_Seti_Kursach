using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Windows;

using CrestiUI.Game;
using CrestiUI.net;
using CrestiUI.Properties;

using Tcp;

namespace CrestiUI
{
    /// <summary>
    ///     Логика взаимодействия для LobbyListWindow.xaml
    /// </summary>
    public partial class LobbyListWindow : Window
    {
        private List<Lobby> lobbyList;


        public LobbyListWindow()
        {
            InitializeComponent();
        }


        //todo сначала мы получаем все ip с открытыс дефолтным портом (8910),
        // потом подключаемся к каждому, и спрашиваем, является ли сервер сервером нашей игры
        // если да, то получаем лобби (наверное надо будет сериализовывать лобби)
        // придется использовать реквесты и скорее всего придется их переписать


        private void updateLobbyList()
        {
            var lobbies = new List<LobbyInLobbyList>();
            var port = Settings.Default.DefaultPort;
            var checkLobbyIps = IPGlobalProperties.GetIPGlobalProperties().GetActiveTcpListeners()
                .Where(l => (l.Port == port) && (l.AddressFamily == AddressFamily.InterNetwork));

            foreach (var ip in checkLobbyIps)
            {
                var lobbyData = getLobbyGetData(ip, port);
                if (lobbyData != null)
                {
                    lobbies.Add(lobbyData);
                }
            }
        }


        private LobbyInLobbyList getLobbyGetData(IPEndPoint ip, int port)
        {
            var asker = new SimpleTcpClient();
            asker.Connect(ip.Address.ToString(), port);
            var ans = asker.WriteLineAndGetReply(new Request("GET", RequestCommands.GetLobbyData.ToString(), null).ToJsonString(), TimeSpan.Zero);
            var response = new Response(ans.MessageString);
            if (response.ResponseArgs["IsLobby"] == "true")
            {
                var lobbyName = response.ResponseArgs["Name"];
                var lobbyIp = response.ResponseArgs["Ip"];
                var lobbyCountOfPlayers = Convert.ToInt32(response.ResponseArgs["CountOfPlayers"]);
                Lobby.LobbyState lobbyState;
                Enum.TryParse(response.ResponseArgs["State"], out lobbyState);

                asker.Disconnect();

                return new LobbyInLobbyList(lobbyName, lobbyIp, lobbyCountOfPlayers, lobbyState);
            }

            asker.Disconnect();

            return null;
        }


        public void ConnectToLobby()
        {
        }
    }
}