# SpotifyWPF
An unofficial, simple tools application for Spotify

This application was born out of a community ask to delete multiple playlists at a time (they had thousands of them).  With that said,
that's the only feature this application supports currently.

The idea is for this to be a sort of "power tools" application for Spotify.  Stuff that you can't do in the app (but can with the public APIs)
will be considered for addition to this app.  Things that you can do in the Spotify app, but cumbersone to perform, will be considersed as
well.

For example:
* Adding all of an artist's tracks to a playlist
* Mass deletion (technically unfollow) of playlists
* Displaying information from the API that the Spotify client doesn't show

If you have any feature ideas, you can either add it as an issue to this repo, or post it on the Spotify Community.  I'll keep a lookout
for any asks I see that the official app doesn't support.

# Installation

If you want to use the [installer](https://mrpnut.github.io/SpotifyWPF/SpotifyWPF.appinstaller), then you need to install my self signed certificate to your computer's trusted certificate authorities.
To do that easily, run the following in an Administrator PowerShell command prompt.  After that, run the installer.

```
Invoke-WebRequest -Uri "https://mrpnut.github.io/SpotifyWPF/SpotifyWPF.cer" -OutFile "$env:temp\SpotifyWPF.cer"
Import-Certificate -FilePath "$env:temp\SpotifyWPF.cer" -CertStoreLocation Cert:\LocalMachine\CA
```

If you don't want to use the installer, you can pick one of the Github releases to extract to a folder and run.
Using the installer will keep SpotifyWPF up to date.

# Running

After installation, you can just search for "SpotifyWPF" in your start menu if you used the installer.
