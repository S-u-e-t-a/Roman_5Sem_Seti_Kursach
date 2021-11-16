using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Windows;

using CrestiUI.Game;

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
            var lobbyIps = IPGlobalProperties.GetIPGlobalProperties().GetActiveTcpListeners();

        }
    }
}