using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CefSharp.Wpf;

namespace Spotify_Display
{
    public static class Config {

        public static ChromiumWebBrowser chromeBrowser;

        public enum MediaCommand : int
        {
            None = 0x0,
            PlayPauseTrack = 0xE0000,
            MuteTrack = 0x80000,
            VolumeDown = 0x90000,
            VolumeUp = 0xA0000,
            StopTrack = 0xD0000,
            PreviousTrack = 0xC0000,
            NextTrack = 0xB0000
        }

        public enum WindowMessage : int
        {
            None = 0x0,
            Hotkey = 0x312,
            AppCommand = 0x319
        }

        public static MediaPlayer Player { get; set; }

        public static MainWindow mainWindow { get; set; }

    }

}
