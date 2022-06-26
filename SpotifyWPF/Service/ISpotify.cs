using SpotifyAPI.Web;
using System;
using System.Threading.Tasks;

namespace SpotifyWPF.Service
{
    public interface ISpotify
    {
        Task LoginAsync(Action onSuccess);

        Task<PrivateUser> GetPrivateProfileAsync();

        ISpotifyClient Api { get; }
    }
}
