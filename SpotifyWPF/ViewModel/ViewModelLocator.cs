using AutoMapper;
using CommonServiceLocator;
using GalaSoft.MvvmLight.Ioc;
using SpotifyWPF.Model;
using SpotifyWPF.Service;
using SpotifyWPF.Service.MessageBoxes;
using SpotifyWPF.ViewModel.Page;

namespace SpotifyWPF.ViewModel
{
    public class ViewModelLocator
    {
        public ViewModelLocator()
        {
            SimpleIoc.Default.Reset();

            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            SimpleIoc.Default.Register(() => AutoMapperConfiguration.Configure().CreateMapper());
            SimpleIoc.Default.Register<ISettingsProvider, SettingsProvider>();
            SimpleIoc.Default.Register<ISpotify, Spotify>();
            SimpleIoc.Default.Register<IMessageBoxService, MessageBoxService>();

            SimpleIoc.Default.Register<MainViewModel>();
            SimpleIoc.Default.Register<LoginPageViewModel>();
            SimpleIoc.Default.Register<PlaylistsPageViewModel>();
            SimpleIoc.Default.Register<SearchPageViewModel>();
        }

        public MainViewModel Main => ServiceLocator.Current.GetInstance<MainViewModel>();

        public PlaylistsPageViewModel PlaylistsPage => ServiceLocator.Current.GetInstance<PlaylistsPageViewModel>();

        public SearchPageViewModel Search => ServiceLocator.Current.GetInstance<SearchPageViewModel>();

        public static void Cleanup()
        {
        }
    }
}