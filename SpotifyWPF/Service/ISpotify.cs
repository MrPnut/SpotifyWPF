using SpotifyAPI.Web;
using SpotifyAPI.Web.Models;
using System;
using System.Threading.Tasks;

namespace SpotifyWPF.Service
{
    public interface ISpotify
    {
        void Login(Action onSuccess);

        Task<PrivateProfile> GetPrivateProfileAsync();

        SpotifyWebAPI Api { get; }
    }
}
