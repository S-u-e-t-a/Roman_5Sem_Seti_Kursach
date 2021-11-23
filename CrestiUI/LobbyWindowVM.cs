using System.Collections.Generic;
using System.Collections.ObjectModel;

using CrestiUI.Game;

using WPF_MVVM_Classes;

namespace CrestiUI
{
    public class LobbyWindowVM : ViewModelBase
    {
        public readonly LocalLobby LocalLobby;

        private RelayCommand _startGame;


        public ObservableCollection<LocalUser> Users
        {
            get { return new(LocalLobby.users); }
            set
            {
                LocalLobby.users = new List<LocalUser>(value);
                OnPropertyChanged();
            }
        }


        public UserInLobby Xplayer
        {
            get { return LocalLobby.Xplayer; }
            set
            {
                LocalLobby.Xplayer = value;
                OnPropertyChanged();
            }
        }

        public UserInLobby Yplayer
        {
            get { return LocalLobby.Yplayer; }
            set
            {
                LocalLobby.Yplayer = value;
                OnPropertyChanged();
            }
        }

        public RelayCommand StartGame
        {
            get { return _startGame ?? (_startGame = new RelayCommand(o => { LocalLobby.SendToServerGameStart(); })); }
        }


        public LobbyWindowVM(LocalLobby lobby)
        {
            LocalLobby = lobby;
            LocalLobby.UsersUpdatedHandler += (sender, args) => { Users = new ObservableCollection<LocalUser>(LocalLobby.users); };
        }
    }
}