using System.Threading.Tasks;
using SpotifyAPI.Web.Enums;
using SpotifyAPI.Web.Models;
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

        private protected override async Task<Paging<FullArtist>> FetchPageInternalAsync()
        {
            var resp = await _spotify.Api.SearchItemsAsync(Query, SearchType.Artist, 20, Items.Count);

            return resp.Artists;
        }
    }
}
