using System.Threading.Tasks;
using SpotifyAPI.Web;
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

        private protected override async Task<Paging<SimplePlaylist, SearchResponse>> FetchPageInternalAsync()
        {
            var req = new SearchRequest(SearchRequest.Types.Playlist, Query)
            {
                Limit = 20,
                Offset = Items.Count
            };

            var resp = await _spotify.Api.Search.Item(req);

            return resp.Playlists;
        }
    }
}
