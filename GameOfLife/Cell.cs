namespace GameOfLife
{
    class Cell
    {
        public int PositionX { get; }
        public int PositionY { get; }
        public int Age { get; private set; }
        public bool IsAlive { get; private set; }

        public Cell(int row, int column, int age, bool isAlive)
        {
            PositionX = row * 5;
            PositionY = column * 5;
            Age = age;
            IsAlive = isAlive;
        }

        public void Die()
        {
            Age = 0;
            IsAlive = false;
        }

        public void IncreaseAge()
        {
            Age += 1;
        }

        public void Revive()
        {
            IsAlive = true;
            Age = 0;
        }
    }
}