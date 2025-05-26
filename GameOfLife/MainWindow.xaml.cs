using System;
using System.Windows;
using System.Windows.Threading;

namespace GameOfLife
{
    public partial class MainWindow : Window
    {
        private Grid mainGrid;
        DispatcherTimer generationTimer;
        private int genCounter;
        private const int maxNumAdWindows = 2;
        private readonly AdWindow[] adWindows = new AdWindow[maxNumAdWindows];

        public MainWindow()
        {
            InitializeComponent();
            mainGrid = new Grid(MainCanvas);

            generationTimer = new DispatcherTimer();
            generationTimer.Tick += OnTimer;
            generationTimer.Interval = TimeSpan.FromMilliseconds(200);
        }

        private void StartAd()
        {
            for (int i = 0; i < adWindows.Length; i++)
            {
                var window = adWindows[i];
                if (window is null || !window.IsLoaded)
                {

                    if (!(window is null))
                    {
                        window.Closed -= AdWindowOnClosed;
                        window.Close();
                    }

                    window = new AdWindow(this);
                    window.Closed += AdWindowOnClosed;
                    window.Top = this.Top + (330 * i) + 70;
                    window.Left = this.Left + 240;
                    window.Show();

                    adWindows[i] = window;
                }
            }
        }

        private void AdWindowOnClosed(object sender, EventArgs eventArgs)
        {
            var closedWindow = sender as AdWindow;
            if (closedWindow == null) return;

            closedWindow.Closed -= AdWindowOnClosed;

            int index = Array.IndexOf(adWindows, closedWindow);
            if (index >= 0)
            {
                adWindows[index] = null;
            }
        }


        private void Button_OnClick(object sender, EventArgs e)
        {
            if (!generationTimer.IsEnabled)
            {
                generationTimer.Start();
                ButtonStart.Content = "Stop";
                StartAd();
            }
            else
            {
                generationTimer.Stop();
                ButtonStart.Content = "Start";
            }
        }

        private void OnTimer(object sender, EventArgs e)
        {
            mainGrid.Update();
            genCounter++;
            lblGenCount.Content = "Generations: " + genCounter;
        }

        private void ButtonClear_Click(object sender, RoutedEventArgs e)
        {
            mainGrid.Clear();
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            if (generationTimer != null)
            {
                generationTimer.Stop();
                generationTimer.Tick -= OnTimer;
                generationTimer = null;
            }
        }
    }
}
