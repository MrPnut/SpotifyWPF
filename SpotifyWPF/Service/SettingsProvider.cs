namespace SpotifyWPF.Service
{
    public class SettingsProvider : ISettingsProvider
    {
        public string SpotifyClientId => Properties.Settings.Default.SpotifyClientId;

        public string SpotifyRedirectPort => Properties.Settings.Default.SpotifyRedirectPort;
    }
}
