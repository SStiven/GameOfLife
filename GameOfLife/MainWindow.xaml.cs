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
                if (adWindows[i] is null || !adWindows[i].IsLoaded)
                {
                    adWindows[i] = new AdWindow(this);
                    adWindows[i].Closed += AdWindowOnClosed;
                    adWindows[i].Top = this.Top + (330 * i) + 70;
                    adWindows[i].Left = this.Left + 240;
                    adWindows[i].Show();
                }
            }
        }

        private void AdWindowOnClosed(object sender, EventArgs eventArgs)
        {
            for (int i = 0; i < adWindows.Length; i++)
            {
                adWindows[i].Closed -= AdWindowOnClosed;
                adWindows[i] = null;
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
    }
}
