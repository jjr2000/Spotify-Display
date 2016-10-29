using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using CefSharp;
using CefSharp.Wpf;
using System.IO;

namespace Spotify_Display
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            InililizeChromium();
            SpotHelp SpotHelp = new SpotHelp();
            SpotHelp.Start();
        }

        public void InililizeChromium()
        {

            String page = string.Format(@"{0}\html-resources\html\index.html", System.AppDomain.CurrentDomain.BaseDirectory);

            if (!File.Exists(page))
            {
                MessageBox.Show("Error The html file doesn't exists : " + page);
            }

            CefSettings settings = new CefSettings();
            settings.PackLoadingDisabled = false;
            if (Cef.Initialize(settings))
            {
                Config.mainWindow = this;
                Config.chromeBrowser = new ChromiumWebBrowser();
                MainGrid.Children.Add(Config.chromeBrowser);
                Config.chromeBrowser.Address = page;

                // Allow the use of local resources in the browser
                BrowserSettings browserSettings = new BrowserSettings();
                browserSettings.FileAccessFromFileUrls = CefState.Enabled;
                browserSettings.UniversalAccessFromFileUrls = CefState.Enabled;
                Config.chromeBrowser.BrowserSettings = browserSettings;
                Config.chromeBrowser.RegisterJsObject("talkbackJs", new TalkbackJs());

            }
        }

        public class TalkbackJs
        {

            bool fullScreen = false;

            public void toggleFullscreen()
            {//Read Note#
                if (fullScreen)
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        Config.mainWindow.WindowStyle = WindowStyle.SingleBorderWindow;
                        Config.mainWindow.WindowState = WindowState.Normal;
                        Config.mainWindow.ResizeMode = ResizeMode.CanResize;
                    });
                    fullScreen = false;
                }
                else
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        Config.mainWindow.WindowState = WindowState.Normal;
                        Config.mainWindow.WindowStyle = WindowStyle.None;
                        Config.mainWindow.WindowState = WindowState.Maximized;
                        Config.mainWindow.ResizeMode = ResizeMode.NoResize;
                    });
                    fullScreen = true;
                }
            }


        }

    }

    class SpotHelp
    {

        public void Start()
        {
            //MediaConfig.PlayerLaunch.Unload();
            Config.Player = new Spotify();
            Config.Player.Load();

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            UpdateLoop();
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

        }

        private async Task UpdateLoop()
        {
            while (true)
            {

                Config.Player.Update();

                // don't run again for at least 200 milliseconds
                await Task.Delay(1000);
            }
        }
    }

}
