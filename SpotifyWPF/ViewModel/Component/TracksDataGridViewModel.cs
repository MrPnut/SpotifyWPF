using System.Threading.Tasks;
using SpotifyAPI.Web;
using SpotifyWPF.Service;

namespace SpotifyWPF.ViewModel.Component
{
    public class TracksDataGridViewModel : DataGridViewModelBase<FullTrack>
    {
        private readonly ISpotify _spotify;

        public TracksDataGridViewModel(ISpotify spotify)
        {
            _spotify = spotify;
        }

        private protected override async Task<Paging<FullTrack, SearchResponse>> FetchPageInternalAsync()
        {
            var req = new SearchRequest(SearchRequest.Types.Track, Query)
            {
                Limit = 20,
                Offset = Items.Count
            };

            var resp = await _spotify.Api.Search.Item(req);

            return resp.Tracks;
        }
    }
}
