using SpotifyAPI.Web;
using SpotifyAPI.Web.Auth;
using SpotifyAPI.Web.Enums;
using SpotifyAPI.Web.Models;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SpotifyWPF.Service
{
    public class Spotify : ISpotify
    {
        private readonly ISettingsProvider _settingsProvider;

        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

        private PrivateProfile _privateProfile;

        public SpotifyWebAPI Api { get; private set; }

        public Spotify(ISettingsProvider settingsProvider)
        {
            _settingsProvider = settingsProvider;
        }

        public void Login(Action onSucces)
        {
            var auth = new ImplicitGrantAuth(
                _settingsProvider.SpotifyClientId, 
                _settingsProvider.SpotifyRedirectUri, 
                _settingsProvider.SpotifyRedirectUri, 
                Scope.UserReadPrivate |                
                Scope.PlaylistModifyPrivate | 
                Scope.PlaylistModifyPublic | 
                Scope.PlaylistReadCollaborative | 
                Scope.PlaylistReadPrivate 
            );

            auth.AuthReceived += (sender, payload) =>
            {
                auth.Stop();

                Api = new SpotifyWebAPI()
                {
                    TokenType = payload.TokenType,
                    AccessToken = payload.AccessToken
                };

                onSucces.Invoke();
            };

            auth.Start();
            auth.OpenBrowser();
        }

        public async Task<PrivateProfile> GetPrivateProfileAsync()
        {            
            if (_privateProfile != null)
            {
                return _privateProfile;
            }

            await _semaphore.WaitAsync();

            try
            {
                if (Api == null)
                {
                    return null;
                }

                _privateProfile = await Api.GetPrivateProfileAsync();

                return _privateProfile;
            }

            finally
            {
                _semaphore.Release();
            }                                                            
        }
    }
}
