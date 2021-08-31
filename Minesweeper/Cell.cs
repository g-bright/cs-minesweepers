using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minesweeper
{
    class Cell
    {
        public int state;
        public bool displayed;
        public bool isBomb;
        public bool selected;
        public int x;
        public int y;
        public int bombCountNear;
        public string displayValue
        {
            get
            {
                if (selected == true)
                    return "@";
                if (!displayed)
                    return "?";
                if (state == 9)
                    return "*";
                if (state == 10)
                    return ("#");
                if (state == 0)
                    return (" ");
                else
                    return state.ToString();
            }
            
        }
        public Cell(int initialValue, int xCoord, int yCoord)
        {
            bombCountNear = 0;
            x = xCoord;
            y = yCoord;
            state = initialValue;
            displayed = false;
        }
    }
}
