using System;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;
using CefSharp;
using CefSharp.Wpf;

namespace Spotify_Display
{
    public static class OutputControl
    {
        private static string lastTextToWrite = string.Empty;


        public static void UpdateTextAndEmptyFilesMaybe(string text)
        {
            if (text != lastTextToWrite)
            {
                lastTextToWrite = text;

                //File.WriteAllText(@Application.StartupPath + @"\Simple.txt", string.Empty);


                //File.WriteAllText(@Application.StartupPath + @"\Album.txt", string.Empty);
                //File.WriteAllText(@Application.StartupPath + @"\Artist.txt", string.Empty);
                //File.WriteAllText(@Application.StartupPath + @"\Track.txt", string.Empty);
                //File.WriteAllText(@Application.StartupPath + @"\TrackId.txt", string.Empty);

                if (Config.chromeBrowser.IsBrowserInitialized && !Config.chromeBrowser.IsDisposed)
                {
                    string script = "ChangeText('" + text + "', '', '');ChangeImage('../images/default.png');";
                    var task = Config.chromeBrowser.EvaluateScriptAsync(script, 1000);
                    task.Wait();
                    var response = task.Result;
                    var result = response.Success ? (response.Result ?? "null") : response.Message;
                }

            }
        }

        public static void UpdateText(string text)
        {
            if (text != lastTextToWrite)
            {
                lastTextToWrite = text;

                // Write the song title and artist to a text file.
                if (Config.chromeBrowser.IsBrowserInitialized && !Config.chromeBrowser.IsDisposed)
                {
                    string script = "ChangeText('" + text + "', '', '');";
                    var task = Config.chromeBrowser.EvaluateScriptAsync(script, 1000);
                    task.Wait();
                    var response = task.Result;
                    var result = response.Success ? (response.Result ?? "null") : response.Message;
                }

            }
        }

        public static void UpdateText(string title, string artist)
        {
            UpdateText(title, artist, string.Empty, string.Empty);
        }

        public static void UpdateText(string title, string artist, string album)
        {
            UpdateText(title, artist, album, string.Empty);
        }

        public static void UpdateText(string title, string artist, string album, string trackId)
        {
            string output = title + " - " + artist + "\r\n" + album;

            if (output != lastTextToWrite)
            {
                lastTextToWrite = output;

                //// Write the song title and artist to a text file.
                //File.WriteAllText(@Application.StartupPath + @"\Simple.txt", output);

                //// Check if we want to save artist and track to separate files.
                //File.WriteAllText(@Application.StartupPath + @"\Artist.txt", artist);
                //File.WriteAllText(@Application.StartupPath + @"\Track.txt", title);
                //File.WriteAllText(@Application.StartupPath + @"\Album.txt", album);
                //File.WriteAllText(@Application.StartupPath + @"\TrackId.txt", trackId);

                if (Config.chromeBrowser.IsBrowserInitialized && !Config.chromeBrowser.IsDisposed)
                {
                    //title = title.Replace("'", "");
                    //artist = artist.Replace("'", "");
                    //album = album.Replace("'", "");

                    string script = "ChangeText(\"" + title + "\", \"" + artist + "\", \"" + album + "\");";
                    var task = Config.chromeBrowser.EvaluateScriptAsync(script, 1000);
                    task.Wait();
                    var response = task.Result;
                    var result = response.Success ? (response.Result ?? "null") : response.Message;
                }


                //// If saving track history is enabled, append that information to a separate file.
                //File.AppendAllText(@Application.StartupPath + @"\History.txt", output + Environment.NewLine);
            }
        }

        public static string UnifyTitles(string title)
        {
            if (!string.IsNullOrEmpty(title))
            {
                // Spotify's search doesn't like all uppercase letters
                // Let's see how all lowercase fairs
                title = title.ToLower(CultureInfo.InvariantCulture);

                // For some unknown reason some versions of Spotify include
                // "Spotify - " before the track information. I doubt this
                // particular string would appear in any sane song title, so
                // let's just remove it.
                title = title.Replace(@"Spotify - ", " ");

                // title = title.Replace(@".", " "); // Causes failed search result from Spotify
                title = title.Replace(@"/", " ");
                title = title.Replace(@"\", " ");
                title = title.Replace(@",", " ");
                // title = title.Replace(@"'", " "); // Causes failed search result from Spotify
                title = title.Replace(@"(", " ");
                title = title.Replace(@")", " ");
                title = title.Replace(@"[", " ");
                title = title.Replace(@"]", " ");
                title = title.Replace(@"!", " ");
                title = title.Replace(@"$", " ");
                title = title.Replace(@"%", " ");
                title = title.Replace(@"&", " ");
                title = title.Replace(@"?", " ");
                title = title.Replace(@":", " ");
                title = title.Replace(@"*", " ");

                title = CompactWhitespace(title);

                return title;
            }
            else
            {
                return string.Empty;
            }
        }

        private static string CompactWhitespace(string text)
        {
            StringBuilder stringBuilder = new StringBuilder(text);

            CompactWhitespace(stringBuilder);

            return stringBuilder.ToString();
        }

        private static void CompactWhitespace(StringBuilder stringBuilder)
        {
            if (stringBuilder.Length == 0)
            {
                return;
            }

            int start = 0;

            while (start < stringBuilder.Length)
            {
                if (char.IsWhiteSpace(stringBuilder[start]))
                {
                    start++;
                }
                else
                {
                    break;
                }
            }

            if (start == stringBuilder.Length)
            {
                stringBuilder.Length = 0;
                return;
            }

            int end = stringBuilder.Length - 1;

            while (end >= 0)
            {
                if (char.IsWhiteSpace(stringBuilder[end]))
                {
                    end--;
                }
                else
                {
                    break;
                }
            }

            int destination = 0;
            bool previousCharIsWhitespace = false;

            for (int i = start; i <= end; i++)
            {
                if (char.IsWhiteSpace(stringBuilder[i]))
                {
                    if (!previousCharIsWhitespace)
                    {
                        previousCharIsWhitespace = true;
                        stringBuilder[destination] = ' ';
                        destination++;
                    }
                }
                else
                {
                    previousCharIsWhitespace = false;
                    stringBuilder[destination] = stringBuilder[i];
                    destination++;
                }
            }

            stringBuilder.Length = destination;
        }
    }
}
