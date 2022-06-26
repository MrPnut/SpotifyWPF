using System.Threading.Tasks;
using SpotifyAPI.Web;
using SpotifyWPF.Service;

namespace SpotifyWPF.ViewModel.Component
{
    public class AlbumsDataGridViewModel : DataGridViewModelBase<SimpleAlbum>
    {
        private readonly ISpotify _spotify;

        public AlbumsDataGridViewModel(ISpotify spotify)
        {
            _spotify = spotify;
        }

        private protected override async Task<Paging<SimpleAlbum, SearchResponse>> FetchPageInternalAsync()
        {
            var req = new SearchRequest(SearchRequest.Types.Album, Query)
            {
                Limit = 20,
                Offset = Items.Count
            };

            var resp = await _spotify.Api.Search.Item(req);

            return resp.Albums;
        }
    }
}
