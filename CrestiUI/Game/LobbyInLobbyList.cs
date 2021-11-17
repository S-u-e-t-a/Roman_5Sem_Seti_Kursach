namespace CrestiUI.Game
{
    public enum LobbyState
    {
        SearchingForPlayers,
        GameStarted
    }

    public class LobbyInLobbyList // todo RENAME
    {
        public string LobbyName { get; protected set; }
        public string Ip { get; }
        public virtual int PlayerCount { get; }

        public LobbyState LobbyState { get; protected set; }


        public LobbyInLobbyList(string lobbyName, string ip, int playerCount, LobbyState lobbyState)
        {
            LobbyName = lobbyName;
            Ip = ip;
            PlayerCount = playerCount;
            LobbyState = lobbyState;
        }


        public LobbyInLobbyList()
        {
        }
    }
}