using System.Threading.Tasks;
using SpotifyAPI.Web.Enums;
using SpotifyAPI.Web.Models;
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

        private protected override async Task<Paging<SimpleAlbum>> FetchPageInternalAsync()
        {
            var resp = await _spotify.Api.SearchItemsAsync(Query, SearchType.Album, 20, Items.Count);

            return resp.Albums;
        }
    }
}
