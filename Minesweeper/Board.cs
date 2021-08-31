using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Minesweeper
{
    class Board
    {
        public int RemainingBombs = 0;
        public int RemainingFlags = 0;
        public int BoardSize;
        public int Bombs;
        public bool FirstMovePlayed = false;
        public bool Game = true;
        public string DifficultyInputGlobal;
        public Board()
        {
            NewBoard:
            Game = true;
            RemainingBombs = 0;
            RemainingFlags = 0;
            FirstMovePlayed = false;
            
            Console.Clear();
            Console.Title = "Minesweeper";
            Console.SetWindowSize(41, 25);
            Console.SetBufferSize(41, 25);
            var grid = new List<List<Cell>>();
            var random = new Random();
            Difficulty:
            Console.WriteLine("Choose a difficulty: (Easy, Medium, Hard or Impossible)");
            var difficultyInput = Console.ReadLine();
            double difficulty;
            if (difficultyInput.ToLower() == "easy")
            {
                difficulty = 0.12;
            }
            else if (difficultyInput.ToLower() == "medium")
            {
                difficulty = 0.17;
            }
            else if (difficultyInput.ToLower() == "hard")
            {
                difficulty = 0.22;
            }
            else if (difficultyInput.ToLower() == "impossible")
            {
                difficulty = 0.32;
            }
            else
            {
                Console.WriteLine("Please enter a valid difficulty");
                Thread.Sleep(2500);
                Console.Clear();
                goto Difficulty;
            }
            DifficultyInputGlobal = difficultyInput.ToUpper();
        Retry:
            Console.Clear();
            Console.Write("Enter board size: (min 4, max 30)");
            Console.OutputEncoding = Encoding.UTF8;

            string value = Console.ReadLine();
            bool isValidNumber = int.TryParse(value, out _);
            int number = isValidNumber ? Convert.ToInt32(value) : 0;

            if (!isValidNumber)
            {
                Console.Write("Please enter a valid board size");
                Thread.Sleep(2000);
                goto Retry;
            }
            else if (isValidNumber && (number < 4 || number > 30))
            {
                Console.Write("The number has to be between 4 and 30");
                Thread.Sleep(2000);
                goto Retry;
            }
            

            BoardSize = number;
            for (var i = 0; i < BoardSize; i++)
            {
                var newRow = new List<Cell>();

                for (var j = 0; j < BoardSize; j++)
                {
                    if (random.NextDouble() < difficulty)
                    {
                        newRow.Add(new Cell(9, i, j));
                        RemainingBombs += 1;
                    }
                    else
                        newRow.Add(new Cell(0, i, j));
                }

                grid.Add(newRow);
            }

            if (RemainingBombs == 0)
            {
                var x = random.Next(1, BoardSize - 1);
                var y = random.Next(1, BoardSize - 1);

                RemainingBombs = 1;
                grid[x][y].state = 9;
            }
            Bombs = RemainingBombs;
            RemainingFlags = RemainingBombs;
            UpdateGameTitle();
            SetBoardNeighbours(grid);
            if (BoardSize < 6)
            {
                Console.SetWindowSize(48, Convert.ToInt32(Math.Round(BoardSize * 2.2) + 4));
                Console.SetBufferSize(48, Convert.ToInt32(Math.Round(BoardSize * 2.2) + 4));
            }
            else
            {
                Console.SetWindowSize((BoardSize * 7) + 4, Convert.ToInt32(Math.Round(BoardSize * 2.2) + 4));
                Console.SetBufferSize((BoardSize * 7) + 4, Convert.ToInt32(Math.Round(BoardSize * 2.2) + 4));
            }
            Console.Clear();

            PrintGrid(grid);
            
            while (Game == true)
            {
                Console.WriteLine("Choose C for cell, B for bomb, -B to delete");
                var selection = Console.ReadLine();
                Loop:
                Console.Clear();

                PrintGrid(grid);

                if (selection.ToLower() == "c")
                {
                    var y = GetCoordinate(grid, "x");
                    var x = GetCoordinate(grid, "y");

                    if (y == 0 || x == 0 || y > BoardSize || x > BoardSize)
                    {
                        Console.Write("Please enter valid coordinates");
                        Thread.Sleep(2000);
                        goto Loop;
                    }

                    SelectCell(x - 1, y - 1, grid);
                }
                else if (selection.ToLower() == "b")
                {
                    var y = GetCoordinate(grid, "x");
                    var x = GetCoordinate(grid, "y");

                    if (y == 0 || x == 0 || y > BoardSize || x > BoardSize)
                    {
                        Console.Write("Please enter valid coordinates");
                        Thread.Sleep(2000);
                        goto Loop;
                    }

                    SelectBombCell(grid, x - 1, y - 1);

                }
                else if (selection == "-b" || selection == "-B")

                {
                    var y = GetCoordinate(grid, "x");
                    var x = GetCoordinate(grid, "y");

                    if (y == 0 || x == 0 || y > BoardSize || x > BoardSize)
                    {
                        Console.Write("Please enter valid coordinates");
                        Thread.Sleep(2000);
                        goto Loop;
                    }

                    DeleteBombMarker(grid, x - 1, y - 1);
                }
            }
            Thread.Sleep(5000);
        EndScreen:
            Console.WriteLine("Type 'new' to play again, or 'exit' to stop playing");
            Console.WriteLine("");
            var input = Console.ReadLine();
            if (input.ToLower() == "new")
            {
                goto NewBoard;
            }
            else if (input.ToLower() == "exit")
            {
                Environment.Exit(0);

            }
            else
            {
                Console.Clear();
                goto EndScreen;
            }
        }

        private void SetBoardNeighbours(List<List<Cell>> grid)
        {
            for (var i = 0; i < BoardSize; i++)
            {
                for (var j = 0; j < BoardSize; j++)
                {
                    if (grid[i][j].state != 9)
                    {
                        var neighbours = CellNeighbours(i, j, grid, true);
                        var count = BombNeighbourCount(neighbours);
                        grid[i][j].state = count;
                    }

                }

            }
        }
        private string PrintList(List<Cell> items)
        {
            UpdateGameTitle();
            var message = (string.Join("  ", items.Select(n => n.displayValue.ToString())));
            Console.WriteLine("");
            return message;
        }
        private int GetCoordinate(List<List<Cell>> grid, string xORy)
        {
            Console.WriteLine($"Enter {xORy} coordinate: ");

            string value = Console.ReadLine();
            bool isValidNumber = int.TryParse(value, out _);
            int number = isValidNumber ? Convert.ToInt32(value) : 0;

            if (!isValidNumber)
                return 0;

            Console.Clear();
            PrintGrid(grid);
            return number;
        }
        private string PrintCoordinatesXAxis(int length)
        {
            var xAxis = "   ";
            if (length < 10)
            {
                for (int i = 1; i < length + 1; i++)
                {
                    xAxis = xAxis + $"{i}  ";
                }
            }
            else if (length < 100)
            {
                for (int i = 1; i < 9 + 1; i++)
                {
                    xAxis = xAxis + $"{i}  ";
                }
                for (int i = 10; i < length + 1; i++)
                {
                    xAxis = xAxis + $"{i} ";
                }
            }
            return xAxis;
        }
        private void Win(List<List<Cell>> grid)
        {
            UpdateGameTitle();
            Console.Beep();
            Console.Beep();
            Console.Beep();
            Console.Clear();
            PrintGridEndScreen(grid);
            Thread.Sleep(5000);
            for (int i = 0; i < 5; i++)
            {
                Console.WriteLine("");
                Thread.Sleep(200);
            }
            Console.WriteLine($"You win! There were {Bombs} bombs in this game.");

            for (int i = 0; i < 5; i++)
            {
                Console.WriteLine("");
                Thread.Sleep(200);
            }
            
            Game = false;
        }
        private void Lose(List<List<Cell>> grid)
        {
            UpdateGameTitle();
            Console.Beep();
            Console.Beep();
            Console.Beep();
            Console.Clear();
            PrintGridEndScreen(grid);
            Thread.Sleep(5000);
            for (int i = 0; i < 5; i++)
            {
                Console.WriteLine("");
                Thread.Sleep(200);
            }
            Console.WriteLine($"You have lost! There were {RemainingBombs} bombs in this game.");
            for (int i = 0; i < 5; i++)
            {
                Console.WriteLine("");
                Thread.Sleep(200);
            }
            
            Game = false;
        }

        private void DeleteBombMarker(List<List<Cell>> grid, int x, int y)
        {
            if (RemainingFlags < RemainingBombs)
            {
                if (grid[x][y].state == 9)
                {
                    grid[x][y].isBomb = false;
                    RemainingBombs += 1;
                }
                RemainingFlags += 1;
                grid[x][y].selected = false;
                grid[x][y].isBomb = false;
                Console.Clear();
                PrintGrid(grid);
            }
        }

        private void SelectBombCell(List<List<Cell>> grid, int x, int y)
        {
            if (RemainingFlags > 0 && RemainingBombs > 0)
            {
                if(grid[x][y].displayed == true)
                {
                    goto Jump;
                }
                if (grid[x][y].state == 9)
                {
                    grid[x][y].isBomb = true;
                    RemainingBombs -= 1;
                    
                }
                RemainingFlags -= 1;
                grid[x][y].selected = true;
                Console.Clear();
                PrintGrid(grid);
                if (RemainingBombs == 0)
                {
                    Win(grid);
                }
            Jump:;
            }

        }
        private void SelectCell(int x, int y, List<List<Cell>> grid)
        {
            Random rnd = new Random();
            int xRand;
            int yRand;
            bool found = false;
            var temp = grid[x][y].state;
            if (temp == 9)
            {
                if (FirstMovePlayed == false)
                {
                    while (found == false)
                    {
                        xRand = rnd.Next(1, BoardSize - 1);
                        yRand = rnd.Next(1, BoardSize - 1);
                        if (grid[xRand][yRand].state != 9)
                        {
                            found = true;
                            grid[xRand][yRand].state = 9;
                            grid[x][y].state = 0;
                        }

                    }
                    SetBoardNeighbours(grid);
                    SelectCell(x, y, grid);
                    FirstMovePlayed = true;
                }
                else
                    Lose(grid);
            }
            else
            {
                grid[x][y].state = 10;
                grid[x][y].displayed = true;
                Console.Clear();
                PrintGrid(grid);
                Thread.Sleep(2000);
                grid[x][y].state = temp;
                grid[x][y].displayed = false;
                Console.Clear();
                PrintGrid(grid);
                var play = CellNeighbours(x, y, grid, true);
                SetCells0(x, y, play, grid);
                Console.Clear();
                PrintGrid(grid);
                FirstMovePlayed = true;
            }
            FirstMovePlayed = true;
        }
        private void PrintGrid(List<List<Cell>> grid)
        {
            UpdateGameTitle();
            Console.WriteLine(PrintCoordinatesXAxis(BoardSize));
            var count = 0;

            foreach (var item in grid)
            {
                string message;
                if (count < 9)
                {
                    message = PrintList(item);
                    count += 1;
                    message = $"{count}  {message}";
                    Console.WriteLine(message);
                }
                else
                {
                    message = PrintList(item);
                    count += 1;
                    message = $"{count} {message}";
                    Console.WriteLine(message);
                }

            }
            count = 1;
        }
        private void SetCells0(int x, int y, List<Cell> neighbours, List<List<Cell>> grid)
        {
            var me = grid[x][y];

            if (me.state != 9)
                me.displayed = true;

            if (me.selected == true)
            {
                me.selected = false;
                RemainingFlags += 1;
            }

            if (me.state > 0)
                return;

            foreach (var neighbour in neighbours)
            {
                if (!neighbour.displayed)
                {
                    var neighbours2 = CellNeighbours(neighbour.x, neighbour.y, grid, true);
                    SetCells0(neighbour.x, neighbour.y, neighbours2, grid);
                }
            }
        }


        private int BombNeighbourCount(List<Cell> cells)
        {
            var count = 0;
            foreach (var item in cells)
            {
                if (item.state == 9)
                {
                    count += 1;
                }
            }
            return count;
        }
        private List<Cell> CellNeighbours(int x, int y, List<List<Cell>> grid, bool diagonals)
        {
            List<Cell> searchList = new List<Cell>();
            try
            {
                searchList.Add(grid[x - 1][y]);

            }
            catch (ArgumentOutOfRangeException)
            {
            }

            try
            {
                searchList.Add(grid[x][y - 1]);
            }
            catch (ArgumentOutOfRangeException)
            {
            }

            try
            {
                searchList.Add(grid[x + 1][y]);
            }
            catch (ArgumentOutOfRangeException)
            {
            }

            try
            {
                searchList.Add(grid[x][y + 1]);
            }
            catch (ArgumentOutOfRangeException)
            {

            }

            if (!diagonals)
                return searchList;

            try
            {
                searchList.Add(grid[x - 1][y - 1]);
            }
            catch (ArgumentOutOfRangeException)
            {
            }

            try
            {
                searchList.Add(grid[x + 1][y - 1]);
            }
            catch (ArgumentOutOfRangeException)
            {
            }

            try
            {
                searchList.Add(grid[x - 1][y + 1]);
            }
            catch (ArgumentOutOfRangeException)
            {
            }

            try
            {
                searchList.Add(grid[x + 1][y + 1]);
            }
            catch (ArgumentOutOfRangeException)
            {
            }

            return searchList;

        }
        private void PrintGridEndScreen(List<List<Cell>> grid)
        {
            Console.WriteLine(PrintCoordinatesXAxis(BoardSize));
            var count = 0;
            foreach (var item in grid)
            {
                foreach (var cell in item)
                {
                    cell.selected = false;
                }
                string message;
                if (count < 9)
                {
                    message = PrintListEndScreen(item);
                    count += 1;
                    message = $"{count}  {message}";
                    Console.WriteLine(message);
                }
                else
                {
                    message = PrintListEndScreen(item);
                    count += 1;
                    message = $"{count} {message}";
                    Console.WriteLine(message);
                }

            }
            count = 1;
        }
        private string PrintListEndScreen(List<Cell> items)
        {
            foreach (var item in items)
            {
                item.displayed = true;
            }

            var message = (string.Join("  ", items.Select(n => n.displayValue.ToString())));
            Console.WriteLine("");
            return message;
        }
        private void UpdateGameTitle()
        {
            Console.Title = $"Minesweeper - {DifficultyInputGlobal} - Flags remaining: {RemainingFlags}";

        }
    }
}
