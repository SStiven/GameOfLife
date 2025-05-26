using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace GameOfLife
{
    class AdWindow : Window
    {
        private readonly DispatcherTimer adTimer;
        private int imageIndexCurrentlyShown;
        private static Random rnd = new Random();
        
        private string[] adUrls = new[]
        {
            "http://example.com/ad1",
            "http://example.com/ad2",
            "http://example.com/ad3"
        };

        private string[] imagePaths = new[]
        {
            "ad1.jpg",
            "ad2.jpg",
            "ad3.jpg"
        };

        private string adUrl;
        private readonly BitmapImage[] adBitmapImages;
        private readonly ImageBrush adBrush = new ImageBrush();


        public AdWindow(Window owner)
        {
            adBitmapImages = new BitmapImage[imagePaths.Length];
            for (int i = 0; i < imagePaths.Length; i++)
            {
                adBitmapImages[i] = LoadBitmap(imagePaths[i]);
            }

            Owner = owner;
            Width = 350;
            Height = 100;
            ResizeMode = ResizeMode.NoResize;
            WindowStyle = WindowStyle.ToolWindow;
            Title = "Support us by clicking the ads";
            Cursor = Cursors.Hand;
            ShowActivated = false;
            MouseDown += OnClick;

            imageIndexCurrentlyShown = rnd.Next(0, adBitmapImages.Length);
            ChangeAds(this, new EventArgs());

            adTimer = InitializeAdTimer();
        }

        private DispatcherTimer InitializeAdTimer()
        {
            var timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(3);
            timer.Tick += ChangeAds;
            timer.Start();

            return timer;
        }

        private void OnClick(object sender, MouseButtonEventArgs e)
        {
            System.Diagnostics.Process.Start(adUrl);
            Close();
        }

        protected override void OnClosed(EventArgs e)
        {
            adTimer.Stop();
            Unsubscribe();
            base.OnClosed(e);
        }

        public void Unsubscribe()
        {
            adTimer.Tick -= ChangeAds;
        }

        private BitmapImage LoadBitmap(string path)
        {
            var bmp = new BitmapImage();
            bmp.BeginInit();
            bmp.UriSource = new Uri(path, UriKind.Relative);
            bmp.CacheOption = BitmapCacheOption.OnLoad;
            bmp.EndInit();
            return bmp;
        }

        private void ChangeAds(object sender, EventArgs eventArgs)
        {

            imageIndexCurrentlyShown = (imageIndexCurrentlyShown + 1) % adBitmapImages.Length;
            adBrush.ImageSource = adBitmapImages[imageIndexCurrentlyShown];
            Background = adBrush;
            adUrl = adUrls[imageIndexCurrentlyShown];
        }
    }
}