using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using SpotifyWPF.Service;

namespace SpotifyWPF.ViewModel.Page
{
    public class LoginPageViewModel : ViewModelBase
    {
        private readonly ISpotify _spotify;
        public RelayCommand SpotifyLoginCommand { get; private set; }

        public LoginPageViewModel(ISpotify spotify)
        {
            _spotify = spotify;

            SpotifyLoginCommand = new RelayCommand(Login);
        }

        public void Login()
        {
            _spotify.Login(OnSuccess);
        }

        private void OnSuccess()
        {
            MessengerInstance.Send<object>(new { }, MessageType.LoginSuccessful);
        }
    }
}
