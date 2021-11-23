using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;

using CrestiUI.Game;

using WPF_MVVM_Classes;

namespace CrestiUI
{
    public class LobbyWindowVM : ViewModelBase
    {
        private readonly LocalLobby _localLobby;


        public ObservableCollection<LocalUser> Users
        {
            get { return new(_localLobby.users); }
            set
            {
                _localLobby.users = new List<LocalUser>(value);
                OnPropertyChanged();
            }
        }


        public UserInLobby Xplayer
        {
            get { return _localLobby.Xplayer; }
            set
            {
                _localLobby.Xplayer = value;
                OnPropertyChanged();
            }
        }

        public UserInLobby Yplayer
        {
            get { return _localLobby.Yplayer; }
            set
            {
                _localLobby.Yplayer = value;
                OnPropertyChanged();
            }
        }


        public LobbyWindowVM(LocalLobby lobby)
        {
            _localLobby = lobby;
            _localLobby.UsersUpdatedHandler += (sender, args) => { Users = new ObservableCollection<LocalUser>(_localLobby.users); };
        }


        private void printUserList(List<LocalUser> users)
        {
            foreach (var user in users)
            {
                Trace.WriteLine(user.Name);
            }
        }
    }
}