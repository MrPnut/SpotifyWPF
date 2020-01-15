using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using SpotifyWPF.ViewModel.Component;
using SpotifyWPF.ViewModel.Page;

namespace SpotifyWPF.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        private readonly SearchPageViewModel _searchPageViewModel;
        private readonly PlaylistsPageViewModel _playlistsPageViewModel;

        private ViewModelBase _currentPage;

        public MainViewModel(LoginPageViewModel loginPageViewModel,
            PlaylistsPageViewModel playlistsPageViewModel,
            SearchPageViewModel searchPageViewModel)
        {
            _playlistsPageViewModel = playlistsPageViewModel;
            _searchPageViewModel = searchPageViewModel;

            CurrentPage = loginPageViewModel;

            MessengerInstance.Register<object>(this, MessageType.LoginSuccessful, LoginSuccessful);

            MenuItems = new ObservableCollection<MenuItemViewModel>
            {
                new MenuItemViewModel("File")
                {
                    MenuItems = new ObservableCollection<MenuItemViewModel>
                    {
                        new MenuItemViewModel("Exit", new RelayCommand(Exit))
                    }
                },
                new MenuItemViewModel("View", new RelayCommand<MenuItemViewModel>(SwitchViewFromMenuItem))
                {
                    MenuItems = new ObservableCollection<MenuItemViewModel>
                    {
                        new MenuItemViewModel("Search",
                            new RelayCommand<MenuItemViewModel>(SwitchViewFromMenuItem)) {IsChecked = true},
                        new MenuItemViewModel("Playlists",
                            new RelayCommand<MenuItemViewModel>(SwitchViewFromMenuItem)),
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
            CurrentPage = _searchPageViewModel;
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

        private static void Exit()
        {
            Application.Current.MainWindow?.Close();
        }
    }
}