using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using SpotifyWPF.Service;
// ReSharper disable AsyncVoidLambda

namespace SpotifyWPF.ViewModel.Page
{
    public class LoginPageViewModel : ViewModelBase
    {
        public RelayCommand SpotifyLoginCommand { get; private set; }

        public LoginPageViewModel(ISpotify spotify)
        {
            SpotifyLoginCommand = new RelayCommand(async () => { await spotify.LoginAsync(OnSuccess); });
        }

        private void OnSuccess()
        {
            MessengerInstance.Send<object>(new { }, MessageType.LoginSuccessful);
        }
    }
}
