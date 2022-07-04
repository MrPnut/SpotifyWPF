using System.Threading.Tasks;
using SpotifyAPI.Web;
using SpotifyWPF.Service;

namespace SpotifyWPF.ViewModel.Component
{
    public class ArtistsDataGridViewModel : DataGridViewModelBase<FullArtist>
    {
        private readonly ISpotify _spotify;
        
        public ArtistsDataGridViewModel(ISpotify spotify)
        {
            _spotify = spotify;
        }

        private protected override async Task<Paging<FullArtist, SearchResponse>> FetchPageInternalAsync()
        {
            var req = new SearchRequest(SearchRequest.Types.Artist, Query)
            {
                Limit = 20,
                Offset = Items.Count
            };

            var resp = await _spotify.Api.Search.Item(req);

            return resp.Artists;
        }
    }
}
