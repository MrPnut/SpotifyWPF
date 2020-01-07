using GalaSoft.MvvmLight;
using SpotifyWPF.Service;

namespace SpotifyWPF.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        private readonly ISpotify _spotify;

        private readonly LoginPageViewModel _loginPageViewModel;

        private readonly PlaylistsViewModel _playlistsViewModel;

        private ViewModelBase _currentPage;

        public ViewModelBase CurrentPage
        {
            get
            {
                return _currentPage; 
            }

            set
            {
                _currentPage = value;
                RaisePropertyChanged();
            }
        }
        
        public MainViewModel(ISpotify spotify, 
                            LoginPageViewModel loginPageViewModel,
                            PlaylistsViewModel playlistsViewModel)
        {
            _spotify = spotify;
            _loginPageViewModel = loginPageViewModel;
            _playlistsViewModel = playlistsViewModel;

            CurrentPage = loginPageViewModel;

            MessengerInstance.Register<object>(this, MessageType.LoginSuccessful, LoginSuccessful);            
        }        

        public async void LoginSuccessful(object o)
        {
            CurrentPage = _playlistsViewModel;
            await _playlistsViewModel.LoadPlaylistsAsync();
        }
    }
}