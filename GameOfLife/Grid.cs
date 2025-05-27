using System;

namespace GameOfLife
{
    public class Grid
    {
        public int Width { get; }
        public int Height { get; }

        private Cell[,] cells;
        private Cell[,] nextGenerationCells;
        private static Random rnd;

        public Grid(int width, int height)
        {
            rnd = new Random();
            Width = width;
            Height = height;
            cells = new Cell[Width, Height];
            nextGenerationCells = new Cell[Width, Height];

            InitializeCells();
        }

        private void InitializeCells()
        {
            for (int i = 0; i < Width; i++)
            {
                for (int j = 0; j < Height; j++)
                {
                    cells[i, j] = new Cell(i, j, 0, false);
                    nextGenerationCells[i, j] = new Cell(i, j, 0, false);
                }
            }

            SetRandomPattern();
        }

        public void Clear()
        {
            for (int i = 0; i < Width; i++)
            {
                for (int j = 0; j < Height; j++)
                {
                    cells[i, j].Die();
                    nextGenerationCells[i, j].Die();
                }
            }
        }

        public static bool GetRandomBoolean()
        {
            return rnd.NextDouble() > 0.8;
        }

        public void SetRandomPattern()
        {
            for (int i = 0; i < Width; i++)
            {
                for (int j = 0; j < Height; j++)
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
            for (int i = 0; i < Width; i++)
            {
                for (int j = 0; j < Height; j++)
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
            }
        }


        public void Update()
        {
            for (int i = 0; i < Width; i++)
            {
                for (int j = 0; j < Height; j++)
                {
                    nextGenerationCells[i, j] = CalculateNextGeneration(i, j);
                }
            }
            UpdateToNextGeneration();
        }

        internal Cell CalculateNextGeneration(int row, int column)
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

        internal int CountNeighbors(int i, int j)
        {
            int count = 0;

            if (i != Width - 1 && cells[i + 1, j].IsAlive) count++;
            if (i != Width - 1 && j != Height - 1 && cells[i + 1, j + 1].IsAlive) count++;
            if (j != Height - 1 && cells[i, j + 1].IsAlive) count++;
            if (i != 0 && j != Height - 1 && cells[i - 1, j + 1].IsAlive) count++;
            if (i != 0 && cells[i - 1, j].IsAlive) count++;
            if (i != 0 && j != 0 && cells[i - 1, j - 1].IsAlive) count++;
            if (j != 0 && cells[i, j - 1].IsAlive) count++;
            if (i != Width - 1 && j != 0 && cells[i + 1, j - 1].IsAlive) count++;

            return count;
        }

        internal bool IsAlive(int i, int j)
        {
            var cell = cells[i, j];
            return cell.IsAlive;
        }

        internal int Age(int i, int j)
        {
            var cell = cells[i, j];
            return cell.Age;
        }

        internal void Revive(int i, int j)
        {
            var cell = cells[i, j];
            cell.Revive();
        }

        internal void Die(int i, int j)
        {
            var cell = cells[i, j];
            cell.Die();
        }
    }
}