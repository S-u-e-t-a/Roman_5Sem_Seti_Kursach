using System.Collections.Generic;
using System.Collections.ObjectModel;

using CRESTI;

using CrestiUI.Game;

using WPF_MVVM_Classes;

namespace CrestiUI
{
    public class LobbyWindowVM : ViewModelBase
    {
        public readonly LocalLobby LocalLobby;

        private bool _canUserBecomeOPlayer;

        private bool _canUserBecomeXPlayer;

        private RelayCommand _makeLocalUserOPlayer;

        private RelayCommand _makeLocalUserXPlayer;

        private RelayCommand _startGame;


        private RelayCommand _writeToChat;

        private string _message;

        public bool CanUserBecomeOPlayer
        {
            get { return _canUserBecomeOPlayer; }
            set
            {
                _canUserBecomeOPlayer = value;
                OnPropertyChanged();
            }
        }

        public bool CanUserBecomeXPlayer
        {
            get { return _canUserBecomeXPlayer; }
            set
            {
                _canUserBecomeXPlayer = value;
                OnPropertyChanged();
            }
        }

        public IEnumerable<string> Localps
        {
            get { return LocalLobby.LocalIps; }
        }

        public LocalUser OPlayer
        {
            get { return LocalLobby.Oplayer; }
            set
            {
                LocalLobby.Oplayer = value;
                OnPropertyChanged();
            }
        }

        public LocalUser XPlayer
        {
            get { return LocalLobby.Xplayer; }
            set
            {
                LocalLobby.Xplayer = value;
                OnPropertyChanged();
            }
        }


        public ObservableCollection<LocalUser> Users
        {
            get { return new(LocalLobby.users); }
            set
            {
                LocalLobby.users = new List<LocalUser>(value);
                OnPropertyChanged();
            }
        }

        public RelayCommand MakeLocalUserOPlayer
        {
            get { return _makeLocalUserOPlayer ?? (_makeLocalUserOPlayer = new RelayCommand(o => { LocalLobby.MakeLocalUserPlayer(PlayerType.O); })); }
        }

        public RelayCommand MakeLocalUserXPlayer
        {
            get { return _makeLocalUserXPlayer ?? (_makeLocalUserXPlayer = new RelayCommand(o => { LocalLobby.MakeLocalUserPlayer(PlayerType.X); })); }
        }

        public RelayCommand StartGame
        {
            get { return _startGame ?? (_startGame = new RelayCommand(o => { LocalLobby.SendToServerGameStart(); })); }
        }

        public string Chat
        {
            get { return LocalLobby.ChatHistory; }
        }

        public string Message
        {
            get { return _message; }
            set
            {
                _message = value;
                OnPropertyChanged();
            }
        }


        public LobbyWindowVM(LocalLobby lobby)
        {
            CanUserBecomeOPlayer = true;
            CanUserBecomeXPlayer = true;
            LocalLobby = lobby;
            LocalLobby.UsersUpdatedHandler += (sender, args) => { OnPropertyChanged(nameof(Users)); };
            LocalLobby.PlayerUpdatedHandler += (sender, args) =>
            {
                OnPropertyChanged(nameof(XPlayer));
                OnPropertyChanged(nameof(OPlayer));
                if (OPlayer != null)
                {
                    CanUserBecomeOPlayer = false;
                }

                if (XPlayer != null)
                {
                    CanUserBecomeXPlayer = false;
                }
            };
            LocalLobby.WritedToChat += (sender, args, username, message) => { OnPropertyChanged(nameof(Chat)); };
        }
    }
}