//using System;
//using System.Collections.Generic;
//using System.Diagnostics;
//using System.Linq;
//using System.Net;
//using System.Net.NetworkInformation;
//using System.Windows;

//using CrestiUI.Game;
//using CrestiUI.net;
//using CrestiUI.Properties;

//using Tcp;

//namespace CrestiUI
//{
//    /// <summary>
//    ///     Логика взаимодействия для LobbyListWindow.xaml
//    /// </summary>
//    public partial class LobbyListWindow : Window
//    {
//        public List<LobbyInLobbyList> LobbyList { get; set; }


//        public LobbyListWindow()
//        {
//            InitializeComponent();
//            updateLobbyList();
//            foreach (var lobby in LobbyList)
//            {
//                Trace.WriteLine(lobby.Ip);
//            }
//        }


//        //todo сначала мы получаем все ip с открытыс дефолтным портом (8910),
//        // потом подключаемся к каждому, и спрашиваем, является ли сервер сервером нашей игры
//        // если да, то получаем лобби (наверное надо будет сериализовывать лобби)
//        // придется использовать реквесты и скорее всего придется их переписать


//        private void updateLobbyList()
//        {
//            var lobbies = new List<LobbyInLobbyList>();
//            var port = Settings.Default.DefaultPort;
//            var checkLobbyIps = IPGlobalProperties.GetIPGlobalProperties().GetActiveTcpListeners()
//                .Where(l => l.Port == port);

//            foreach (var ip in checkLobbyIps)
//            {
//                var lobbyData = getLobbyGetData(ip, port);
//                if (lobbyData != null)
//                {
//                    lobbies.Add(lobbyData);
//                }
//            }

//            lobbies.Add(new LobbyInLobbyList("sss", "26.34.42.3", 1, LobbyState.SearchingForPlayers));
//            LobbyList = lobbies;
//            LobbyDataGrid.ItemsSource = LobbyList;
//        }


//        private LobbyInLobbyList getLobbyGetData(IPEndPoint ip, int port)
//        {
//            var asker = new SimpleTcpClient();
//            asker.Connect(ip.Address.ToString(), port);
//            var ans = asker.WriteLineAndGetReply(new Request("GET", RequestCommands.GETLobbyData, null).ToJsonString(), TimeSpan.FromSeconds(3));
//            if (ans != null)
//            {
//                var response = new Response(ans.MessageString);
//                if (response.Args["IsLobby"] == "true")
//                {
//                    var lobbyName = response.Args["Name"];
//                    var lobbyIp = response.Args["Ip"];
//                    var lobbyCountOfPlayers = Convert.ToInt32(response.Args["CountOfPlayers"]);
//                    LobbyState lobbyState;
//                    Enum.TryParse(response.Args["State"], out lobbyState);

//                    asker.Disconnect();

//                    return new LobbyInLobbyList(lobbyName, lobbyIp, lobbyCountOfPlayers, lobbyState);
//                }

//                asker.Disconnect();
//            }

//            return null;
//        }


//        public void ConnectToLobby(LobbyInLobbyList lobby)
//        {
//            //var joinLobbyWindow = new JoinLobbyWindow(lobby);
//            //joinLobbyWindow.Show();

//            Close();
//        }


//        private void Connect_Click(object sender, RoutedEventArgs e)
//        {
//            //var lobby = LobbyDataGrid.SelectedItem as LobbyInLobbyList;
//            //var joinLobbyWindow = new JoinLobbyWindow(lobby);
//            //joinLobbyWindow.Show();
//            Close();
//        }


//        private void Create_Click(object sender, RoutedEventArgs e)
//        {
//            var createLobbyWindow = new CreateLobbyWindow();
//            createLobbyWindow.Show();
//            Close();
//        }
//    }
//}

