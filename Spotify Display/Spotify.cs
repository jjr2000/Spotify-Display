using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spotify_Display
{
    using System;
    using System.ComponentModel;
    using System.Globalization;
    using System.IO;
    using System.Net;
    using System.Web;
    using SimpleJson;
    using CefSharp;
    internal sealed class Spotify : MediaPlayer
    {
        private string json = string.Empty;
        private bool downloadingJson = false;

        public override void Update()
        {
            if (!this.Found)
            {
                this.Handle = UnsafeNativeMethods.FindWindow("SpotifyMainWindow", null);

                this.Found = true;
                this.NotRunning = false;
            }
            else
            {
                // Make sure the process is still valid.
                if (this.Handle != IntPtr.Zero && this.Handle != null)
                {
                    int windowTextLength = UnsafeNativeMethods.GetWindowText(this.Handle, this.Title, this.Title.Capacity);

                    string spotifyTitle = this.Title.ToString();

                    this.Title.Clear();

                    // If the window title length is 0 then the process handle is not valid.
                    if (windowTextLength > 0)
                    {
                        // Only update if the title has actually changed.
                        // This prevents unnecessary calls and downloads.
                        if (spotifyTitle != this.LastTitle)
                        {

                            if (spotifyTitle == "Spotify")
                            {
                                this.SaveBlankImage();

                                OutputControl.UpdateTextAndEmptyFilesMaybe("No Track Is Currently Playing");
                            }
                            else
                            {
                                this.DownloadJson(spotifyTitle);

                                if (!string.IsNullOrEmpty(this.json))
                                {
                                    dynamic jsonSummary = SimpleJson.DeserializeObject(this.json);

                                    if (jsonSummary != null)
                                    {
                                        var numberOfResults = jsonSummary.tracks.total;

                                        if (numberOfResults > 0)
                                        {
                                            jsonSummary = SimpleJson.DeserializeObject(jsonSummary.tracks["items"].ToString());

                                            int mostPopular = SelectTrackByPopularity(jsonSummary, spotifyTitle);

                                            OutputControl.UpdateText(
                                                jsonSummary[mostPopular].name.ToString(),
                                                jsonSummary[mostPopular].artists[0].name.ToString(),
                                                jsonSummary[mostPopular].album.name.ToString(),
                                                jsonSummary[mostPopular].id.ToString());

                                            this.DownloadSpotifyAlbumArtwork(jsonSummary[mostPopular].album);
                                        }
                                        else
                                        {
                                            string temptitle = spotifyTitle.Split('(')[0].Trim();

                                            temptitle = ReplaceString(temptitle, "radio", "", StringComparison.CurrentCultureIgnoreCase);
                                            temptitle = ReplaceString(temptitle, "edit", "", StringComparison.CurrentCultureIgnoreCase);

                                            this.DownloadJson(temptitle);

                                            if (!string.IsNullOrEmpty(this.json))
                                            {
                                                dynamic jsonSummary2 = SimpleJson.DeserializeObject(this.json);

                                                if (jsonSummary2 != null)
                                                {
                                                    var numberOfResults2 = jsonSummary2.tracks.total;

                                                    if (numberOfResults2 > 0)
                                                    {
                                                        jsonSummary2 = SimpleJson.DeserializeObject(jsonSummary2.tracks["items"].ToString());

                                                        int mostPopular = SelectTrackByPopularity(jsonSummary2, temptitle);

                                                        OutputControl.UpdateText(
                                                            jsonSummary2[mostPopular].name.ToString(),
                                                            jsonSummary2[mostPopular].artists[0].name.ToString(),
                                                            jsonSummary2[mostPopular].album.name.ToString(),
                                                            jsonSummary2[mostPopular].id.ToString());

                                                        this.DownloadSpotifyAlbumArtwork(jsonSummary2[mostPopular].album);
                                                    }
                                                    else
                                                    {
                                                        // In the event of an advertisement (or any song that returns 0 results)
                                                        // then we'll just write the whole title as a single string instead.
                                                        OutputControl.UpdateTextAndEmptyFilesMaybe(spotifyTitle);
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                // For whatever reason the JSON file couldn't download
                                                // In the event this happens we'll just display Spotify's window title as the track
                                                OutputControl.UpdateTextAndEmptyFilesMaybe(spotifyTitle);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        string temptitle = spotifyTitle.Split('(')[0].Trim();

                                        temptitle = ReplaceString(temptitle, "radio", "", StringComparison.CurrentCultureIgnoreCase);
                                        temptitle = ReplaceString(temptitle, "edit", "", StringComparison.CurrentCultureIgnoreCase);

                                        this.DownloadJson(temptitle);

                                        if (!string.IsNullOrEmpty(this.json))
                                        {
                                            dynamic jsonSummary2 = SimpleJson.DeserializeObject(this.json);

                                            if (jsonSummary2 != null)
                                            {
                                                var numberOfResults2 = jsonSummary2.tracks.total;

                                                if (numberOfResults2 > 0)
                                                {
                                                    jsonSummary2 = SimpleJson.DeserializeObject(jsonSummary2.tracks["items"].ToString());

                                                    int mostPopular = SelectTrackByPopularity(jsonSummary2, temptitle);

                                                    OutputControl.UpdateText(
                                                        jsonSummary2[mostPopular].name.ToString(),
                                                        jsonSummary2[mostPopular].artists[0].name.ToString(),
                                                        jsonSummary2[mostPopular].album.name.ToString(),
                                                        jsonSummary2[mostPopular].id.ToString());

                                                    this.DownloadSpotifyAlbumArtwork(jsonSummary2[mostPopular].album);
                                                }
                                                else
                                                {
                                                    // In the event of an advertisement (or any song that returns 0 results)
                                                    // then we'll just write the whole title as a single string instead.
                                                    OutputControl.UpdateTextAndEmptyFilesMaybe(spotifyTitle);
                                                }
                                            }
                                        }
                                        else
                                        {
                                            // For whatever reason the JSON file couldn't download
                                            // In the event this happens we'll just display Spotify's window title as the track
                                            OutputControl.UpdateTextAndEmptyFilesMaybe(spotifyTitle);
                                        }
                                    }
                                }
                                else
                                {
                                    string temptitle = spotifyTitle.Split('(')[0].Trim();

                                    temptitle = ReplaceString(temptitle, "radio", "", StringComparison.CurrentCultureIgnoreCase);
                                    temptitle = ReplaceString(temptitle, "edit", "", StringComparison.CurrentCultureIgnoreCase);

                                    this.DownloadJson(temptitle);

                                    if (!string.IsNullOrEmpty(this.json))
                                    {
                                        dynamic jsonSummary2 = SimpleJson.DeserializeObject(this.json);

                                        if (jsonSummary2 != null)
                                        {
                                            var numberOfResults2 = jsonSummary2.tracks.total;

                                            if (numberOfResults2 > 0)
                                            {
                                                jsonSummary2 = SimpleJson.DeserializeObject(jsonSummary2.tracks["items"].ToString());

                                                int mostPopular = SelectTrackByPopularity(jsonSummary2, temptitle);

                                                OutputControl.UpdateText(
                                                    jsonSummary2[mostPopular].name.ToString(),
                                                    jsonSummary2[mostPopular].artists[0].name.ToString(),
                                                    jsonSummary2[mostPopular].album.name.ToString(),
                                                    jsonSummary2[mostPopular].id.ToString());

                                                this.DownloadSpotifyAlbumArtwork(jsonSummary2[mostPopular].album);
                                            }
                                            else
                                            {
                                                // In the event of an advertisement (or any song that returns 0 results)
                                                // then we'll just write the whole title as a single string instead.
                                                OutputControl.UpdateTextAndEmptyFilesMaybe(spotifyTitle);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        // For whatever reason the JSON file couldn't download
                                        // In the event this happens we'll just display Spotify's window title as the track
                                        OutputControl.UpdateTextAndEmptyFilesMaybe(spotifyTitle);
                                    }
                                }
                            }

                            this.LastTitle = spotifyTitle;
                        }
                    }
                    else
                    {
                        if (!this.NotRunning)
                        {
                            this.ResetSinceSpotifyIsNotRunning();
                        }
                    }
                }
                else
                {
                    if (!this.NotRunning)
                    {
                        this.ResetSinceSpotifyIsNotRunning();
                    }
                }
            }
        }

        public static string ReplaceString(string str, string oldValue, string newValue, StringComparison comparison)
        {
            StringBuilder sb = new StringBuilder();

            int previousIndex = 0;
            int index = str.IndexOf(oldValue, comparison);
            while (index != -1)
            {
                sb.Append(str.Substring(previousIndex, index - previousIndex));
                sb.Append(newValue);
                index += oldValue.Length;

                previousIndex = index;
                index = str.IndexOf(oldValue, index, comparison);
            }
            sb.Append(str.Substring(previousIndex));

            return sb.ToString();
        }

        public override void Unload()
        {
            base.Unload();
        }

        public override void ChangeToNextTrack()
        {
            UnsafeNativeMethods.SendMessage(this.Handle, (uint)Config.WindowMessage.AppCommand, IntPtr.Zero, new IntPtr((long)Config.MediaCommand.NextTrack));
        }

        public override void ChangeToPreviousTrack()
        {
            UnsafeNativeMethods.SendMessage(this.Handle, (uint)Config.WindowMessage.AppCommand, IntPtr.Zero, new IntPtr((long)Config.MediaCommand.PreviousTrack));
        }

        public override void IncreasePlayerVolume()
        {
            UnsafeNativeMethods.SendMessage(this.Handle, (uint)Config.WindowMessage.AppCommand, IntPtr.Zero, new IntPtr((long)Config.MediaCommand.VolumeUp));
        }

        public override void DecreasePlayerVolume()
        {
            UnsafeNativeMethods.SendMessage(this.Handle, (uint)Config.WindowMessage.AppCommand, IntPtr.Zero, new IntPtr((long)Config.MediaCommand.VolumeDown));
        }

        public override void MutePlayerAudio()
        {
            UnsafeNativeMethods.SendMessage(this.Handle, (uint)Config.WindowMessage.AppCommand, IntPtr.Zero, new IntPtr((long)Config.MediaCommand.MuteTrack));
        }

        public override void PlayOrPauseTrack()
        {
            UnsafeNativeMethods.SendMessage(this.Handle, (uint)Config.WindowMessage.AppCommand, IntPtr.Zero, new IntPtr((long)Config.MediaCommand.PlayPauseTrack));
        }

        public override void StopTrack()
        {
            UnsafeNativeMethods.SendMessage(this.Handle, (uint)Config.WindowMessage.AppCommand, IntPtr.Zero, new IntPtr((long)Config.MediaCommand.StopTrack));
        }

        private void ResetSinceSpotifyIsNotRunning()
        {
            if (!this.SavedBlankImage)
            {
                this.SaveBlankImage();
            }

            OutputControl.UpdateTextAndEmptyFilesMaybe("Please start Spotify to use this app");

            this.Found = false;
            this.NotRunning = true;
        }

        private void DownloadJson(string spotifyTitle)
        {
            // Prevent redownloading JSON if it's already attempting to
            if (!this.downloadingJson)
            {
                this.downloadingJson = true;

                using (WebClient jsonWebClient = new WebClient())
                {
                    try
                    {
                        // There are certain characters that can cause issues with Spotify's search
                        //spotifyTitle = OutputControl.UnifyTitles(spotifyTitle);

                        jsonWebClient.Encoding = System.Text.Encoding.UTF8;

                        var downloadedJson = jsonWebClient.DownloadString(
                            string.Format(
                                CultureInfo.InvariantCulture,
                                "https://api.spotify.com/v1/search?q={0}&type=track",
                                HttpUtility.UrlEncode(spotifyTitle)));

                        if (!string.IsNullOrEmpty(downloadedJson))
                        {
                            this.json = downloadedJson;
                        }
                    }
                    catch (WebException)
                    {
                        this.json = string.Empty;
                        this.SaveBlankImage();
                    }
                }

                this.downloadingJson = false;
            }
        }

        private static int SelectTrackByPopularity(dynamic jsonSummary, string windowTitle)
        {
            long highestPopularity = 0;

            int currentKey = 0;
            int keyWithHighestPopularity = 0;

            foreach (dynamic track in jsonSummary)
            {
                if (windowTitle.Contains(track.artists[0].name) && windowTitle.Contains(track.name))
                {
                    if (track.popularity > highestPopularity)
                    {
                        highestPopularity = track.popularity;
                        keyWithHighestPopularity = currentKey;
                    }
                }

                currentKey++;
            }

            return keyWithHighestPopularity;
        }

        private void DownloadSpotifyAlbumArtwork(dynamic jsonSummary)
        {
            string albumId = jsonSummary.id.ToString();

            // This assumes that the Spotify image array will always have three results (which in all of my tests it has so far)
            string imageUrl = string.Empty;
            imageUrl = jsonSummary.images[0].url.ToString();

            if (Config.chromeBrowser.IsBrowserInitialized && !Config.chromeBrowser.IsDisposed)
            {
                string script = "ChangeImage('" + imageUrl + "');";
                var task = Config.chromeBrowser.EvaluateScriptAsync(script, 1000);
                task.Wait();
                var response = task.Result;
                var result = response.Success ? (response.Result ?? "null") : response.Message;
            }

        }

        private void DownloadSpotifyFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                try
                {
                    File.Copy((string)e.UserState, this.DefaultArtworkFilePath, true);
                }
                catch (IOException)
                {
                    this.SaveBlankImage();
                }
            }
        }

        private class WebClientWithShortTimeout : WebClient
        {
            // How many seconds before webclient times out and moves on.
            private const int WebClientTimeoutSeconds = 10;

            protected override WebRequest GetWebRequest(Uri address)
            {
                WebRequest webRequest = base.GetWebRequest(address);
                webRequest.Timeout = WebClientTimeoutSeconds * 60 * 1000;
                return webRequest;
            }
        }
    }
}
