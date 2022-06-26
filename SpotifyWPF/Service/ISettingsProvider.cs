namespace SpotifyWPF.Service
{
    public interface ISettingsProvider
    {
        string SpotifyClientId { get; }

        string SpotifyRedirectPort { get; }
    }
}
