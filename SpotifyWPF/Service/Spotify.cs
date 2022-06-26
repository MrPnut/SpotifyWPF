using SpotifyAPI.Web;
using SpotifyAPI.Web.Auth;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SpotifyWPF.Service
{
    public class Spotify : ISpotify
    {
        private readonly ISettingsProvider _settingsProvider;

        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

        private readonly EmbedIOAuthServer _server;

        private PrivateUser _privateProfile;

        private Action _loginSuccessAction;

        public ISpotifyClient Api { get; private set; }

        public Spotify(ISettingsProvider settingsProvider)
        {
            _settingsProvider = settingsProvider;

            _server = new EmbedIOAuthServer(
                new Uri($"http://localhost:{_settingsProvider.SpotifyRedirectPort}"),
                int.Parse(_settingsProvider.SpotifyRedirectPort));

            _server.ImplictGrantReceived += OnImplicitGrantReceived;
            _server.ErrorReceived += OnErrorReceived;
        }

        public async Task LoginAsync(Action onSuccess)
        {
            await _server.Start();

            _loginSuccessAction = onSuccess;

            var request = new LoginRequest(_server.BaseUri, _settingsProvider.SpotifyClientId,
                LoginRequest.ResponseType.Token)
            {
                Scope = new List<string>
                {
                    Scopes.UserReadPrivate, Scopes.PlaylistModifyPrivate, Scopes.PlaylistModifyPublic,
                    Scopes.PlaylistReadCollaborative, Scopes.PlaylistReadPrivate
                }
            };
             
            BrowserUtil.Open(request.ToUri());
        }

        private async Task OnErrorReceived(object sender, string error, string state)
        {
            await _server.Stop();
        }

        private async Task OnImplicitGrantReceived(object arg1, ImplictGrantResponse arg2)
        {
            await _server.Stop();

            Api = new SpotifyClient(arg2.AccessToken);
            
            _loginSuccessAction?.Invoke();
        }

        public async Task<PrivateUser> GetPrivateProfileAsync()
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

                if (_privateProfile != null)
                {
                    return _privateProfile;
                }

                _privateProfile = await Api.UserProfile.Current();

                return _privateProfile;
            }

            finally
            {
                _semaphore.Release();
            }
        }
    }
}
