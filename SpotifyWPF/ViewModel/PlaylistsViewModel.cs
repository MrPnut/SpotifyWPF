using AutoMapper;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using SpotifyAPI.Web.Models;
using SpotifyWPF.Model;
using SpotifyWPF.Service;
using SpotifyWPF.Service.MessageBoxes;
using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace SpotifyWPF.ViewModel
{
    public class PlaylistsViewModel : ViewModelBase
    {
        private readonly ISpotify _spotify;

        private readonly IMapper _mapper;

        private readonly IMessageBoxService _messageBoxService;

        public ObservableCollection<SimplePlaylist> Playlists { get; } = new ObservableCollection<SimplePlaylist>();

        public ObservableCollection<Track> Tracks { get; } = new ObservableCollection<Track>();

        private string _status = "Ready";

        public string Status
        {
            get
            {
                return _status;
            }

            set
            {
                _status = value;
                RaisePropertyChanged();

                if (value == "Ready")
                {
                    ProgressVisibility = Visibility.Hidden;
                } 
                
                else
                {
                    ProgressVisibility = Visibility.Visible;
                }
            }
        }

        private Visibility _progressVisibility = Visibility.Hidden;

        public Visibility ProgressVisibility
        {
            get
            {
                return _progressVisibility;
            }

            set
            {
                _progressVisibility = value;
                RaisePropertyChanged();
            }
        }

        public RelayCommand<SimplePlaylist> LoadTracksCommand { get; private set; }

        public RelayCommand<IList> DeletePlaylistsCommand { get; private set; }

        public PlaylistsViewModel(ISpotify spotify, IMapper mapper, IMessageBoxService messageBoxService)
        {
            _spotify = spotify;
            _mapper = mapper;
            _messageBoxService = messageBoxService;

            LoadTracksCommand = new RelayCommand<SimplePlaylist>(LoadTracksAsync);
            DeletePlaylistsCommand = new RelayCommand<IList>(DeletePlaylistsAsync);
        }

        public async void DeletePlaylistsAsync(IList items)
        {            
            var playlists = items.Cast<SimplePlaylist>();

            if (playlists.Count() <= 0)
            {
                return;
            }

            var message = playlists.Count() == 1 ? $"Are you sure you want to delete playlist {playlists.ElementAt(0).Name}?" :
                $"Are you sure you want to delete these {playlists.Count()} playlists?";

            var result = _messageBoxService.ShowMessageBox(
                message, 
                "Confirm", 
                Service.MessageBoxes.MessageBoxButton.YesNo,
                MessageBoxIcon.Exclamation
            );

            if (result != Service.MessageBoxes.MessageBoxResult.Yes)
            {
                return;
            }
                                    
            for (var i = playlists.Count() - 1; i >= 0; i--)
            {
                var playlist = playlists.ElementAt(i);

                Status = "Deleting playlist: " + playlist.Name;

                await _spotify.Api.UnfollowPlaylistAsync(playlist.Owner.Id, playlist.Id);

                await Application.Current.Dispatcher.BeginInvoke((Action)(() => {                    
                    Playlists.Remove(playlist);
                }));
            }

            Status = "Ready";
        }

        public async Task LoadPlaylistsAsync()
        {
            Status = "Loading playlists...";

            Playlists.Clear();

            var profile = await _spotify.GetPrivateProfileAsync();

            Paging<SimplePlaylist> playlists = await _spotify.Api.GetUserPlaylistsAsync(profile.Id);            
            var received = 0;

            do
            {
                received += playlists.Items.Count;

                await Application.Current.Dispatcher.BeginInvoke((Action)(() => { AddPlaylists(playlists); }));

                if (received < playlists.Total)
                {
                    playlists = await _spotify.Api.GetUserPlaylistsAsync(profile.Id, 20, received);
                }

            } while (received < playlists.Total);

                        
            Status = "Ready";
        }

        private void AddPlaylists(Paging<SimplePlaylist> playlists)
        {
            foreach (var playlist in playlists.Items)
            {
                Playlists.Add(playlist);
            }
        }

        public async void LoadTracksAsync(SimplePlaylist playlist)
        {
            Status = "Loading tracks...";

            Tracks.Clear();
            
            Paging<PlaylistTrack> tracks = await GetPlaylistTracksAsync(playlist.Id, 0);            
            var received = 0;

            do
            {
                received += tracks.Items.Count;

                await Application.Current.Dispatcher.BeginInvoke((Action)(() => { AddTracks(tracks); }));
                                 
                if (received < tracks.Total)
                {                
                    tracks = await GetPlaylistTracksAsync(playlist.Id, received);
                }

            } while (received < tracks.Total);

            Status = "Ready";
        }

        private async Task<Paging<PlaylistTrack>> GetPlaylistTracksAsync(string playlistId, int offset)
        {
            // The warning is actually for a different method than the one used
#pragma warning disable CS0618 // Type or member is obsolete
            return await _spotify.Api.GetPlaylistTracksAsync(playlistId, "", 100, offset, "");
#pragma warning restore CS0618 // Type or member is obsolete
        }

        private void AddTracks(Paging<PlaylistTrack> tracks)
        {
            foreach (var track in tracks.Items)
            {
                Tracks.Add(_mapper.Map<Track>(track));                
            }
        }
    }
}
