using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace UwuForms
{
    public class ScreenBuffer
    {
        const char CR = '\x0d';
        const char LF = '\x0a';

        bool dirty;

        int rows = 0;
        int cols = 0;
        Cell[] screen;
        ScreenPos cursor;
        Color activeFgColor;
        Color activeBgColor;

        public Color ActiveFgColor
        {
            get { return activeFgColor; }
        }
        public Color ActiveBgColor
        {
            get { return activeBgColor; }
        }

        public int Rows
        {
            get { return rows; }
        }
        public int Cols
        {
            get { return cols; }
        }

        public struct Cell
        {
            public char ch;
            public Color bgColor;
            public Color fgColor;
            public bool dirty;

            public Cell(char ch, Color bgColor, Color fgColor, bool dirty)
            {
                this.ch = ch;
                this.bgColor = bgColor;
                this.fgColor = fgColor;
                this.dirty = dirty;
            }
        }

        public ScreenPos Cursor
        {
            get { return cursor; }
        }

        public void SetColor(Color fg, Color bg)
        {
            activeFgColor = fg;
            activeBgColor = bg;
        }

        public struct ScreenPos
        {
            public int r;
            public int c;
            public ScreenPos(int r=0, int c=0)
            {
                this.r = r;
                this.c = c;
            }
        }

        public ScreenBuffer()
        {
            this.rows = 0; this.cols = 0;
            screen = null;
            activeBgColor = Color.White;
            activeFgColor = Color.Black;
        }

        public void Resize(int nrows, int ncols)
        {
            dirty = true;
            var nScreen = new Cell[nrows * ncols];
            int roff = nrows - rows;
            ScreenPos npos = new ScreenPos();
            ScreenPos opos = new ScreenPos();
            for (npos.r = 0; npos.r < nrows; npos.r++)
            {
                //Console.WriteLine($"r={npos.r}");
                for (npos.c = 0; npos.c < ncols; npos.c++)
                {
                    opos.c = npos.c;
                    opos.r = npos.r - roff;
                    
                    Cell cell;
                    if (opos.r >= 0 && opos.r < rows && opos.c < cols)
                    {                     
                        cell = screen[Index(opos)];
                        cell.dirty = true;  
                    } else {
                        cell = new Cell(' ', ActiveBgColor, ActiveFgColor, true);
                    }
                    nScreen[Index(npos, nrows, ncols)] = cell;
                }
            }
            rows = nrows;
            cols = ncols;
            screen = nScreen;
        }

        public void ClearScreen()
        {
            ScreenPos pos;
            for (pos.r = 0; pos.r < rows; pos.r++)
            {
                for (pos.c = 0; pos.c < cols; pos.c++)
                {
                    SetChar(pos, ' ');
                }
            }
            MoveCursor(new ScreenPos(0, 0));
        }


        int Index(ScreenPos pos)
        {
            return Index(pos, Rows, Cols);
        }

        int Index(ScreenPos pos, int rows, int cols)
        {
            return pos.r * cols + pos.c;
        }

        public void SetChar(ScreenPos pos, char ch)
        {
            int idx = Index(pos);
            var c = new Cell(ch, activeBgColor, activeFgColor, true);
            screen[idx] = c;
            dirty = true;
        }

        public void MakeDirty(ScreenPos pos)
        {
            int idx = Index(cursor);
            screen[idx].dirty = true;
            dirty = true;
        }

        public void MoveCursor(ScreenPos pos)
        {
            MakeDirty(cursor);
            if (pos.c >= cols)
            { 
                pos.r++; pos.c = 0;
            }
            if (pos.c < 0)
            {
                pos.r--; pos.c = cols - 1;
            }
            if (pos.r >= rows)
            {
                ScrollUp();
                pos.r = rows - 1;
            }
            if (pos.r < 0)
            {
                ScrollDown();
                pos.r = 0;
            }

            cursor = pos;
            MakeDirty(cursor);
        }

        public void ClearRow(int row)
        {
            var pos = new ScreenPos(row, 0);
            for (pos.c = 0; pos.c < cols; pos.c++)
            {
                SetChar(pos, ' ');
            }
        }

        public void FullScreenRefresh()
        {
            dirty = true;
            for (int i = 0; i < screen.Length; i++)
            {
                screen[i].dirty = true;
            }
        }

        public void ScrollUp()
        {
            Array.Copy(screen, cols, screen, 0, (rows - 1) * cols);
            ClearRow(rows - 1);
            FullScreenRefresh();
        }

        public void ScrollDown()
        {
            Array.Copy(screen, 0, screen, cols, (rows - 1) * cols);
            ClearRow(0);
            FullScreenRefresh();
        }


        //public delegate void DrawHandler(object sender, int r, int c, char ch, Color bgColor, Color fgColor);
        //public event DrawHandler Draw;
        //protected void OnDraw(ScreenPos pos, int idx)
        //{
        //    Cell c = screen[idx];
        //    Draw?.Invoke(this, pos.r, pos.c, c.ch, c.bgColor, c.fgColor);
        //}

        public IEnumerable<(ScreenPos pos, Cell cell)> RefreshScreen()
        {
            ScreenPos raster;
            if (dirty)
            {
                for (raster.c = 0; raster.c < cols; raster.c++)
                {
                    for (raster.r = 0; raster.r < rows; raster.r++)
                    {
                        int idx = Index(raster);
                        if (screen[idx].dirty)
                        {
                            screen[idx].dirty = false;
                            yield return (raster, screen[idx]);
                        }
                    }
                }
            }
            dirty = false;
        }


        public void Print(char ch)
        {
            var npos = cursor;
            if (ch == CR)
            {
                npos.c = 0;
                MoveCursor(npos);
            }
            else if (ch == LF)
            {
                npos.r++;
                MoveCursor(npos);
            }
            else
            {
                SetChar(cursor, ch);
                npos.c++;
                MoveCursor(npos);
            }
        }


    }
}
