using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace GameOfLife
{
    internal class GridRenderer
    {
        private readonly Canvas canvas;
        private readonly Ellipse[,] ellipses;
        private readonly int cellSize;
        private readonly Grid grid;

        public GridRenderer(Canvas canvas, Grid grid, int cellSize)
        {
            this.canvas = canvas;
            this.grid = grid;
            this.cellSize = cellSize;

            int width = (int)(canvas.Width / cellSize);
            int height = (int)(canvas.Height / cellSize);
            ellipses = new Ellipse[width, height];

            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    var e = new Ellipse
                    {
                        Width = cellSize,
                        Height = cellSize,
                        Fill = Brushes.Gray,
                        Margin = new Thickness(i * cellSize, j * cellSize, 0, 0)
                    };

                    e.MouseMove += MouseMove;
                    e.MouseLeftButtonDown += MouseMove;
                    canvas.Children.Add(e);

                    ellipses[i, j] = e;
                }
            }
        }

        void MouseMove(object sender, MouseEventArgs e)
        {
            var cellVisual = sender as Ellipse;

            int i = (int)cellVisual.Margin.Left / cellSize;
            int j = (int)cellVisual.Margin.Top / cellSize;

            if (e.LeftButton == MouseButtonState.Pressed)
            {
                if (!grid.IsAlive(i, j))
                {
                    grid.Revive(i, j);
                    cellVisual.Fill = Brushes.White;
                }
                else
                {
                    grid.Die(i, j);
                    cellVisual.Fill = Brushes.Gray;
                }
            }
        }

        public void Update()
        {
            for (int i = 0; i < grid.Width; i++)
            {
                for (int j = 0; j < grid.Height; j++)
                {
                    var isAlive = grid.IsAlive(i, j);
                    int age = grid.Age(i, j);

                    SolidColorBrush newColor;

                    if (isAlive)
                    {
                        newColor = age < 2 ? Brushes.White : Brushes.DarkGray;
                    }
                    else
                    {
                        newColor = Brushes.Gray;
                    }

                    ellipses[i, j].Fill = newColor;
                }
            }
        }
    }
}
