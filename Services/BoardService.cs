using Chess.Classes;
using System.Collections.Generic;

namespace Chess.Services
{
    public class BoardService
    {
        readonly List<char> colChars = new List<char>() { ' ', 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', ' ' };
        readonly List<Cell> cells = new List<Cell>();

        public BoardService()
        {
            // Create 10 Rows
            for (int row = 9; row >= 0; row--)
            {
                // Create 10 Cols
                for (int col = 0; col <= 9; col++)
                {
                    CreateCell(col, row);
                }
            }
        }

        public List<Cell> GetCells()
        {
            return cells;
        }

        private void CreateCell(int col, int row)
        {
            Cell cell = new Cell
            {
                Id = 0,
                Name = "cell" + col + row
            };

            // Playground
            //cell.Text
            //cell.CssBackgroundColor
            if (row < 9 && row > 0 && col > 0 && col < 9)
            {
                cell.Text = "";
                // Set Cell Colors
                if ((row % 2 == 0 && col % 2 == 0) || (row % 2 == 1 && col % 2 == 1))
                {
                    cell.CssBackgroundColor = "brown";
                }
                else
                {
                    cell.CssBackgroundColor = "white";
                }
            }
            else
            // Outside of the playground, information area
            {
                cell.CssBackgroundColor = "gray";
                // Set row names to info area
                if ((col == 0 || col == 9) && row > 0 && row < 9)
                {
                    cell.Text = row.ToString();
                }
                // Set col numbers to info area
                else if ((row == 0 || row == 9) && col > 0 && col < 9)
                {
                    cell.Text = colChars[col].ToString();
                }
                // Set corner cells names
                else
                {
                    cell.Text = "";
                }
            }

            cells.Add(cell);
        }
    }
}