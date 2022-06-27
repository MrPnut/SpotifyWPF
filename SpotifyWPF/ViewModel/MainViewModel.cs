using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using SpotifyWPF.Service.MessageBoxes;
using SpotifyWPF.ViewModel.Component;
using SpotifyWPF.ViewModel.Page;
using MessageBoxButton = SpotifyWPF.Service.MessageBoxes.MessageBoxButton;

namespace SpotifyWPF.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        private readonly SearchPageViewModel _searchPageViewModel;
        private readonly PlaylistsPageViewModel _playlistsPageViewModel;
        private readonly LoginPageViewModel _loginPageViewModel;

        private readonly IMessageBoxService _messageBoxService;

        private ViewModelBase _currentPage;

        public MainViewModel(LoginPageViewModel loginPageViewModel,
            PlaylistsPageViewModel playlistsPageViewModel,
            SearchPageViewModel searchPageViewModel,
            IMessageBoxService messageBoxService)
        {
            _playlistsPageViewModel = playlistsPageViewModel;
            _searchPageViewModel = searchPageViewModel;
            _loginPageViewModel = loginPageViewModel;

            _messageBoxService = messageBoxService;

            CurrentPage = loginPageViewModel;

            MessengerInstance.Register<object>(this, MessageType.LoginSuccessful, LoginSuccessful);

            MenuItems = new ObservableCollection<MenuItemViewModel>
            {
                new MenuItemViewModel("File")
                {
                    MenuItems = new ObservableCollection<MenuItemViewModel>
                    {
                        new MenuItemViewModel("Log Out", new RelayCommand(Logout)),
                        new MenuItemViewModel("Exit", new RelayCommand(Exit))
                    }
                },
                new MenuItemViewModel("View", new RelayCommand<MenuItemViewModel>(SwitchViewFromMenuItem))
                {
                    MenuItems = new ObservableCollection<MenuItemViewModel>
                    {
                        new MenuItemViewModel("Search",
                            new RelayCommand<MenuItemViewModel>(SwitchViewFromMenuItem)),
                        new MenuItemViewModel("Playlists",
                            new RelayCommand<MenuItemViewModel>(SwitchViewFromMenuItem)) {IsChecked = true},
                    }
                }
            };
        }

        public ObservableCollection<MenuItemViewModel> MenuItems { get; set; }

        public ViewModelBase CurrentPage
        {
            get => _currentPage;

            set
            {
                _currentPage = value;
                RaisePropertyChanged();
            }
        }

        private void LoginSuccessful(object o)
        {
            CurrentPage = _playlistsPageViewModel;
        }

        private void SwitchViewFromMenuItem(MenuItemViewModel menuItem)
        {
            switch (menuItem.Header)
            {
                case "Playlists":
                    CurrentPage = _playlistsPageViewModel;
                    break;
                case "Search":
                    CurrentPage = _searchPageViewModel;
                    break;
                default:
                    return;
            }

            MenuItems.First(item => item.Header == "View")
                .MenuItems.ToList().ForEach(item => item.IsChecked = false);

            menuItem.IsChecked = true;
        }

        private void Logout()
        {
            _messageBoxService.ShowMessageBox(
                "SpotifyWPF will log itself out of Spotify.  Close your browser and/or delete cookies, or you will instantly log back in.", 
                "Log Out", MessageBoxButton.OK, MessageBoxIcon.Asterisk);

            CurrentPage = _loginPageViewModel;
        }

        private static void Exit()
        {
            Application.Current.MainWindow?.Close();
        }
    }
}