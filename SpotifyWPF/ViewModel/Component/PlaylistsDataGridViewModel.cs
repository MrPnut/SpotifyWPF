using System.Threading.Tasks;
using SpotifyAPI.Web.Enums;
using SpotifyAPI.Web.Models;
using SpotifyWPF.Service;

namespace SpotifyWPF.ViewModel.Component
{
    public class PlaylistsDataGridViewModel : DataGridViewModelBase<SimplePlaylist>
    {
        private readonly ISpotify _spotify;

        public PlaylistsDataGridViewModel(ISpotify spotify)
        {
            _spotify = spotify;
        }

        private protected override async Task<Paging<SimplePlaylist>> FetchPageInternalAsync()
        {
            var resp = await _spotify.Api.SearchItemsAsync(Query, SearchType.Playlist, 20, Items.Count);

            return resp.Playlists;
        }
    }
}
