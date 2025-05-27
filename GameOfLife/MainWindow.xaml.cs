using System;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace GameOfLife
{
    public partial class MainWindow : Window
    {
        private const int cellSize = 5;
        private Grid grid;
        private readonly GridRenderer gridRenderer;
        DispatcherTimer generationTimer;
        private int genCounter;
        private const int maxNumAdWindows = 2;
        private readonly AdWindow[] adWindows = new AdWindow[maxNumAdWindows];

        public MainWindow()
        {
            InitializeComponent();

            int width = (int)(MainCanvas.Width / cellSize);
            int height = (int)(MainCanvas.Height / cellSize);

            grid = new Grid(width, height);
            gridRenderer = new GridRenderer(MainCanvas, grid, cellSize);

            gridRenderer.Update();

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
            grid.Update();
            gridRenderer.Update();
            genCounter++;
            lblGenCount.Content = "Generations: " + genCounter;
        }

        private void ButtonClear_Click(object sender, RoutedEventArgs e)
        {
            grid.Clear();
            RepaintCanvas(grid);
        }

        private void RepaintCanvas(Grid grid)
        {
            gridRenderer.Update();
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
