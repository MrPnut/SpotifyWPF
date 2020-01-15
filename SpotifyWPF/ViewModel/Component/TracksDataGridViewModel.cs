using System.Threading.Tasks;
using SpotifyAPI.Web.Enums;
using SpotifyAPI.Web.Models;
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

        private protected override async Task<Paging<FullTrack>> FetchPageInternalAsync()
        {
            var resp = await _spotify.Api.SearchItemsAsync(Query, SearchType.Track, 20, Items.Count);

            return resp.Tracks;
        }
    }
}
