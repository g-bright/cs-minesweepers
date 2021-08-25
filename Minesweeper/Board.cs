﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Minesweeper
{
    class Board
    {
        public int BombCount = 0;
        public int BombLives = 0;
        public int BoardSize = 20;
        public int MineCount = 10;
        public int MarkersPlaced = 0;
        public int Count = 0;
        public bool Game = true;
        public Board()
        {
            Console.SetWindowSize(41, 25);
            Console.SetBufferSize(41, 25);
            var grid = new List<List<Cell>>();
            var random = new Random();
            Console.Write("Enter board size: (min 4, max 30)");
            BoardSize = int.Parse(Console.ReadLine());
            for (var i = 0; i < BoardSize; i++)
            {
                var newRow = new List<Cell>();

                for (var j = 0; j < BoardSize; j++)
                {
                    if (random.NextDouble() < 0.12) 
                    {
                        newRow.Add(new Cell(9, i, j));
                        BombCount += 1;
                        BombLives += 1;
                    }
                    else
                        newRow.Add(new Cell(0, i, j));

                }

                grid.Add(newRow);
            }
            if (BombCount == 0)
            {
                Random rnd = new Random();
                var x = rnd.Next(1, BoardSize - 1);
                var y = rnd.Next(1, BoardSize - 1);
                grid[x][y].state = 9;
            }
            var bombTotal = BombCount;
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

            Console.SetWindowSize((BoardSize * 3) + 4, Convert.ToInt32(Math.Round(BoardSize * 2.2) + 4));
            Console.SetBufferSize((BoardSize * 3) + 4, Convert.ToInt32(Math.Round(BoardSize * 2.2) + 4));

            Console.Title = "Minesweeper";
            Console.Clear();

            PrintGrid(grid);

            while (Game == true)
            {
                Console.WriteLine("Choose C for cell, B for bomb, -B to delete");
                var selection = Console.ReadLine();
                Console.Clear();

                PrintGrid(grid);


                if (selection.ToLower() == "c")
                {
                    var y = GetCoordinate(grid, "x");
                    var x = GetCoordinate(grid, "y");

                    SelectCell(x - 1, y - 1, grid);
                }
                else if (selection.ToLower() == "b")
                {
                    var y = GetCoordinate(grid, "x");
                    var x = GetCoordinate(grid, "y");
                    SelectBombCell(grid, x - 1, y - 1);

                }
                else if (selection == "-b" || selection == "-B")
                {
                    var y = GetCoordinate(grid, "x");
                    var x = GetCoordinate(grid, "y");
                    DeleteBombMarker(grid, x - 1, y - 1, bombTotal);
                }
            }
            Console.ReadLine();

            //-------------------------------------------------------------------------//
        }


        private string PrintList(List<Cell> items)
        {

            //Console.SetWindowSize(235, 69);
            //Console.SetBufferSize(235, 69);
            Console.Title = "Minesweeper";
            var message = (string.Join("  ", items.Select(n => n.displayValue.ToString())));
            Console.WriteLine("");
            return message;
        }
        private int GetCoordinate(List<List<Cell>> grid, string xORy)
        {
            Console.WriteLine($"Enter {xORy} coordinate: ");
            var input = int.Parse(Console.ReadLine());
            Console.Clear();
            PrintGrid(grid);
            return input;
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
            Console.WriteLine($"You win! There were {BombCount} bombs in this game.");

            for (int i = 0; i < 5; i++)
            {
                Console.WriteLine("");
                Thread.Sleep(200);
            }
            Console.ReadLine();
            Environment.Exit(0);
            Game = false;
        }
        private void Lose(List<List<Cell>> grid)
        {
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
            Console.WriteLine($"You have lost! There were {BombCount} bombs in this game.");
            for (int i = 0; i < 5; i++)
            {
                Console.WriteLine("");
                Thread.Sleep(200);
            }
            Console.ReadLine();
            Environment.Exit(0);
            Game = false;
        }

        private void DeleteBombMarker(List<List<Cell>> grid, int x, int y, int bombTotal)
        {
            if (BombLives <= bombTotal)
            {
                if (grid[x][y].state == 9)
                {
                    grid[x][y].isBomb = false;
                    BombCount += 1;

                }
                MarkersPlaced -= 1;
                grid[x][y].selected = false;
                grid[x][y].isBomb = false;
                Console.Clear();
                PrintGrid(grid);
            }
        }

        private void SelectBombCell(List<List<Cell>> grid, int x, int y)
        {
            if (BombLives > 0)
                if (grid[x][y].state == 9)
                {
                    grid[x][y].isBomb = true;
                    BombCount -= 1;
                    if (BombCount == 0)
                    {
                        Win(grid);
                    }
                }
            MarkersPlaced += 1;
            grid[x][y].selected = true;
            Console.Clear();
            PrintGrid(grid);

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
                if (Count == 0)
                {
                    while (found == false)
                    {
                        xRand = rnd.Next(1, BoardSize - 1);
                        yRand = rnd.Next(1, BoardSize - 1);
                        if (grid[xRand][yRand].state == 0)
                        {
                            found = true;
                        }
                        grid[xRand][yRand].state = 9;
                        grid[x][y].state = 0;
                    }
                }
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
                var play = CellNeighbours(x, y, grid, false);
                SetCells0(x, y, play, grid);
                Console.Clear();
                PrintGrid(grid);
                Count += 1;
            }
        }
        private void PrintGrid(List<List<Cell>> grid)
        {
            Console.WriteLine(PrintCoordinatesXAxis(BoardSize));
            Count = 0;
            foreach (var item in grid)
            {
                string message;
                if (Count < 9)
                {
                    message = PrintList(item);
                    Count += 1;
                    message = $"{Count}  {message}";
                    Console.WriteLine(message);
                }
                else
                {
                    message = PrintList(item);
                    Count += 1;
                    message = $"{Count} {message}";
                    Console.WriteLine(message);
                }

            }
            Count = 1;
        }
        private void SetCells0(int x, int y, List<Cell> neighbours, List<List<Cell>> grid)
        {
            var me = grid[x][y];

            if (me.state != 9)
                me.displayed = true;

            if (me.state > 0)
                return;

            foreach (var neighbour in neighbours)
            {
                if (!neighbour.displayed)
                {
                    var neighbours2 = CellNeighbours(neighbour.x, neighbour.y, grid, false);
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
            Count = 0;
            foreach (var item in grid)
            {
                string message;
                if (Count < 9)
                {
                    message = PrintListEndScreen(item);
                    Count += 1;
                    message = $"{Count}  {message}";
                    Console.WriteLine(message);
                }
                else
                {
                    message = PrintListEndScreen(item);
                    Count += 1;
                    message = $"{Count} {message}";
                    Console.WriteLine(message);
                }

            }
            Count = 1;
        }
        private string PrintListEndScreen(List<Cell> items)
        {
            foreach (var item in items)
            {
                item.displayed = true;
            }

            Console.Title = "Minesweeper";
            var message = (string.Join("  ", items.Select(n => n.displayValue.ToString())));
            Console.WriteLine("");
            return message;
        }
    }
}
