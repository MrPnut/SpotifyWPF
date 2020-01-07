namespace SpotifyWPF.Service
{
    public class SettingsProvider : ISettingsProvider
    {
        public string SpotifyClientId => Properties.Settings.Default.SpotifyClientId;

        public string SpotifyRedirectUri => Properties.Settings.Default.SpotifyRedirectUri;
    }
}
