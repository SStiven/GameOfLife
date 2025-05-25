using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace GameOfLife
{
    class Grid
    {
        private int SizeX;
        private int SizeY;
        private Cell[,] cells;
        private Cell[,] nextGenerationCells;
        private static Random rnd;
        private Canvas drawCanvas;
        private Ellipse[,] cellsVisuals;

        public Grid(Canvas c)
        {
            drawCanvas = c;
            rnd = new Random();
            SizeX = (int)(c.Width / 5);
            SizeY = (int)(c.Height / 5);
            cells = new Cell[SizeX, SizeY];
            nextGenerationCells = new Cell[SizeX, SizeY];
            cellsVisuals = new Ellipse[SizeX, SizeY];

            for (int i = 0; i < SizeX; i++)
            {
                for (int j = 0; j < SizeY; j++)
                {
                    cells[i, j] = new Cell(i, j, 0, false);
                    nextGenerationCells[i, j] = new Cell(i, j, 0, false);
                }
            }

            SetRandomPattern();
            InitCellsVisuals();
            UpdateGraphics();
        }

        public void Clear()
        {
            for (int i = 0; i < SizeX; i++)
                for (int j = 0; j < SizeY; j++)
                {
                    cells[i, j] = new Cell(i, j, 0, false);
                    nextGenerationCells[i, j] = new Cell(i, j, 0, false);
                    cellsVisuals[i, j].Fill = Brushes.Gray;
                }
        }

        void MouseMove(object sender, MouseEventArgs e)
        {
            var cellVisual = sender as Ellipse;

            int i = (int)cellVisual.Margin.Left / 5;
            int j = (int)cellVisual.Margin.Top / 5;

            if (e.LeftButton == MouseButtonState.Pressed)
            {
                if (!cells[i, j].IsAlive)
                {
                    cells[i, j].Revive();
                    cellVisual.Fill = Brushes.White;
                }
                else
                {
                    cells[i,j].Die();
                    cellVisual.Fill = Brushes.Gray;
                }
            }
        }

        public void UpdateGraphics()
        {
            for (int i = 0; i < SizeX; i++)
            {
                for (int j = 0; j < SizeY; j++)
                {
                    cellsVisuals[i, j].Fill = cells[i, j].IsAlive
                                                  ? (cells[i, j].Age < 2 ? Brushes.White : Brushes.DarkGray)
                                                  : Brushes.Gray;
                }
            }
        }

        public void InitCellsVisuals()
        {
            for (int i = 0; i < SizeX; i++)
            {
                for (int j = 0; j < SizeY; j++)
                {
                    cellsVisuals[i, j] = new Ellipse();
                    cellsVisuals[i, j].Width = cellsVisuals[i, j].Height = 5;
                    double left = cells[i, j].PositionX;
                    double top = cells[i, j].PositionY;
                    cellsVisuals[i, j].Margin = new Thickness(left, top, 0, 0);
                    cellsVisuals[i, j].Fill = Brushes.Gray;
                    drawCanvas.Children.Add(cellsVisuals[i, j]);

                    cellsVisuals[i, j].MouseMove += MouseMove;
                    cellsVisuals[i, j].MouseLeftButtonDown += MouseMove;
                }
            }
            UpdateGraphics();
        }

        public static bool GetRandomBoolean()
        {
            return rnd.NextDouble() > 0.8;
        }

        public void SetRandomPattern()
        {
            for (int i = 0; i < SizeX; i++)
            {
                for (int j = 0; j < SizeY; j++)
                {
                    var isAlive = GetRandomBoolean();
                    if (isAlive)
                    {
                        cells[i, j].Revive();
                    }
                    else
                    {
                        cells[i, j].Die();
                    }
                }
            }
        }

        public void UpdateToNextGeneration()
        {
            for (int i = 0; i < SizeX; i++)
                for (int j = 0; j < SizeY; j++)
                {
                    if (nextGenerationCells[i, j].IsAlive)
                    {
                        if (cells[i, j].IsAlive)
                        {
                            cells[i, j].IncreaseAge();
                        }
                        else
                        {
                            cells[i, j].Revive();
                        }
                    }
                    else
                    {
                        cells[i, j].Die();
                    }
                }

            UpdateGraphics();
        }


        public void Update()
        {
            for (int i = 0; i < SizeX; i++)
            {
                for (int j = 0; j < SizeY; j++)
                {
                    nextGenerationCells[i, j] = CalculateNextGeneration(i, j);
                }
            }
            UpdateToNextGeneration();
        }

        public Cell CalculateNextGeneration(int row, int column)
        {
            var currentCell = cells[row, column];
            var nextGenerationCell = nextGenerationCells[row, column];
            bool isAlive = currentCell.IsAlive;
            int neighborsCount = CountNeighbors(row, column);

            if (isAlive && neighborsCount < 2)
            {
                nextGenerationCell.Die();
                return nextGenerationCell;
            }

            if (isAlive && (neighborsCount == 2 || neighborsCount == 3))
            {
                nextGenerationCell.IncreaseAge();
                return nextGenerationCell;
            }

            if (isAlive && neighborsCount > 3)
            {
                nextGenerationCell.Die();
                return nextGenerationCell;
            }

            if (!isAlive && neighborsCount == 3)
            {
                nextGenerationCell.Revive();
                return nextGenerationCell;
            }

            nextGenerationCell.Die();
            return nextGenerationCell;
        }

        public int CountNeighbors(int i, int j)
        {
            int count = 0;

            if (i != SizeX - 1 && cells[i + 1, j].IsAlive) count++;
            if (i != SizeX - 1 && j != SizeY - 1 && cells[i + 1, j + 1].IsAlive) count++;
            if (j != SizeY - 1 && cells[i, j + 1].IsAlive) count++;
            if (i != 0 && j != SizeY - 1 && cells[i - 1, j + 1].IsAlive) count++;
            if (i != 0 && cells[i - 1, j].IsAlive) count++;
            if (i != 0 && j != 0 && cells[i - 1, j - 1].IsAlive) count++;
            if (j != 0 && cells[i, j - 1].IsAlive) count++;
            if (i != SizeX - 1 && j != 0 && cells[i + 1, j - 1].IsAlive) count++;

            return count;
        }
    }
}