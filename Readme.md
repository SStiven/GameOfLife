# Task 3 - Game of Live

I used the Visual Studio 2022 tool called Performace Profiler I and got this information

![alt text](memory-profiler-initial.png)

You can notice a lot of memory allocation from different handlers and especially from the GameOfLife.Cell objects, so if we review the Cell class we notice an anemic domain model that contains a hard-coded constant ‘5’ whose meaning we don’t yet know; we’ll try to figure that out later. Coming back to our main issue, since we have so many allocations we need to find where the Cell objects are being created, and I discovered that each time the Grid class generates the next generation, a new Cell instance is allocated—adding significant memory pressure. For now, let’s update the existing cells instead of creating new ones. To do that, we’ll modify the CalculateNextGeneration method in the Grid class and adjust the Cell class accordingly.

The main changes in the Cell class encapsulates the some of the most important logic on it, and provides semantics

```cs
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
```

The main changes to the Cell class encapsulate some of its most important logic and provide clearer semantics.

```cs
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

//......
}
```

We reduced memory allocation, as shown in the following screenshot

![alt text](memory-profiles-remove-creation.png)


We also have in the class MainWindow, that the AdWindow array is created each time StartAdd is called, is better to reuse them and remove some magic numbers like '2' from the original code:

```cs
        private void StartAd()
        {
            adWindow = new AdWindow[2];
            for (int i = 0; i < 2; i++)
            {
                if (adWindow[i] == null)
                {
                    adWindow[i] = new AdWindow(this);
                    adWindow[i].Closed += AdWindowOnClosed;
                    adWindow[i].Top = this.Top + (330 * i) + 70;
                    adWindow[i].Left = this.Left + 240;
                    adWindow[i].Show();
                }
            }
        }
```

I also noticed in MainWindow that the AdWindow array is re-created every time StartAd is called—and it hard-codes the number 2. It’s cleaner to reuse a fixed array and remove those magic numbers:

```cs
        public AdWindow(Window owner)
        {
            Random rnd = new Random();
            Owner = owner;
            Width = 350;
            Height = 100;
            ResizeMode = ResizeMode.NoResize;
            WindowStyle = WindowStyle.ToolWindow;
            Title = "Support us by clicking the ads";
            Cursor = Cursors.Hand;
            ShowActivated = false;
            MouseDown += OnClick;
            
            imageNumberCurrentlyShown = rnd.Next(1, 3);
            ChangeAds(this, new EventArgs());

            // Run the timer that changes the ad's image 
            adTimer = new DispatcherTimer();
            adTimer.Interval = TimeSpan.FromSeconds(3);
            adTimer.Tick += ChangeAds;
            adTimer.Start();
        }
```

In AdWindow, the timer was never stopped or unsubscribed when the window closed, causing a leak, and I also fixed it by overriding OnClosed:

```cs
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
```

I also discovered that a new BitmapImage was created on every tick—hitting the disk repeatedly and leaking memory. To solve this, we preload each image once in the constructor:


```cs
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
```

We had been allocating a brand-new ImageBrush on every ad swap. Now we create a single ImageBrush up front and reuse it; similarly, each BitmapImage is loaded exactly once, so each 3-second tick only swaps indexes instead of re-reading files or instantiating objects.

As an extra improvement, I could introduce a persistence layer or caching service dedicated to loading and managing ad 
images, separating those concerns from the window itself.

## Come back to MainWindow class

In the constructor of the MainWindow class we a time subscription that is not unsubcribed

```cs
        public MainWindow()
        {
            InitializeComponent();
            mainGrid = new Grid(MainCanvas);

            generationTimer = new DispatcherTimer();
            generationTimer.Tick += OnTimer;
            generationTimer.Interval = TimeSpan.FromMilliseconds(200);
        }
```

The original code doesn't stop or deatach OnTimer, so generationTimer maintains a reference to MainWindows, in this way it is never garbage-collecter if is closed. For this reason, I overrided the OnClosed method

```cs
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
```




## ToDos

After refactoring the Grid and Cell classes, I have a few concerns:

- The Grid class has too many responsibilities, perhaps splitting its functionality into separate classes would be beneficial.

- Generation updates still run on the UI thread, we should move them off the UI thread once the application is in a more stable state.

- It might make sense to encapsulate the logic that decides whether a cell should revive, die, or age within the Cell class itself.

- Introduce a persistence layer for async image loading 

- We may not need two separate grids (one for current cells and one for the next generation), but I’m not sure.