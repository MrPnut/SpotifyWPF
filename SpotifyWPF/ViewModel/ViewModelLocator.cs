using CommonServiceLocator;
using GalaSoft.MvvmLight.Ioc;
using SpotifyWPF.Model;
using SpotifyWPF.Service;
using SpotifyWPF.Service.MessageBoxes;

namespace SpotifyWPF.ViewModel
{
    public class ViewModelLocator
    {
        public ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            SimpleIoc.Default.Register(() => { return AutoMapperConfiguration.Configure().CreateMapper(); });
            SimpleIoc.Default.Register<ISettingsProvider, SettingsProvider>();
            SimpleIoc.Default.Register<ISpotify, Spotify>();
            SimpleIoc.Default.Register<IMessageBoxService, MessageBoxService>();
            
            SimpleIoc.Default.Register<MainViewModel>();
            SimpleIoc.Default.Register<LoginPageViewModel>();
            SimpleIoc.Default.Register<PlaylistsViewModel>();
        }

        public MainViewModel Main
        {
            get
            {
                return ServiceLocator.Current.GetInstance<MainViewModel>();
            }
        }

        public PlaylistsViewModel Playlists
        {
            get
            {
                return ServiceLocator.Current.GetInstance<PlaylistsViewModel>();
            }
        }
        
        public static void Cleanup()
        {            
        }
    }
}