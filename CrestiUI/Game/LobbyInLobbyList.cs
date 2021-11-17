using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrestiUI.Game
{
    class LobbyInLobbyList // todo RENAME
    {
        public string LobbyName { get; }
        public string Ip { get;}
        public int PlayerCount { get; }

        public Lobby.LobbyState LobbyState { get; }


        public LobbyInLobbyList(string lobbyName, string ip, int playerCount, Lobby.LobbyState lobbyState)
        {
            LobbyName = lobbyName;
            Ip = ip;
            PlayerCount = playerCount;
            LobbyState = lobbyState;
        }
    }
}
