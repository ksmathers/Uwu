using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UwuForms
{
    public partial class Terminal : UserControl
    {

        public ScreenBuffer.ScreenPos ScreenSize()
        {
            return new ScreenBuffer.ScreenPos(screen.Rows, screen.Cols);
        }

        public int Rows { get { return screen.Rows; } }
        public int Cols { get { return screen.Cols; } }

        ScreenBuffer screen = new ScreenBuffer();
        Font textFont;
        Bitmap page1;
        Color screenBackground = Color.White;
        Color screenForeground = Color.Black;
        bool dirty = false;

        SizeF fTextMetricsM;
        Size iTextMetricsM;

        public delegate void KeyInHandler(object sender, Keys k, bool shift, bool alt, bool ctrl);
        public event KeyInHandler KeyIn;

        public Terminal()
        {
            this.PreviewKeyDown += Terminal_PreviewKeyDown;
            InitializeComponent();
            screenBuffer.HandleCreated += ScreenBuffer_HandleCreated;
            textFont = null;
            screenRefreshTimer.Enabled = true;
            screenRefreshTimer.Interval = 10;
            screenRefreshTimer.Start();
        }

        private void Terminal_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            OnKeyIn(e.KeyCode, e.Shift, e.Alt, e.Control);
        }

        private void MakeDirty()
        {
            dirty = true;
        }

        public void SetColors(Color fg, Color bg)
        {
            screen.SetColor(fg, bg);
        }

        void OnKeyIn(Keys k, bool Shift, bool Alt, bool Ctrl)
        {
            KeyIn?.Invoke(this, k, Shift, Alt, Ctrl);
        }

        public void SetFont(string pattern)
        {
            foreach (var ff in FontFamily.Families)
            {
                if (ff.Name.Contains(pattern))
                {
                    SetFont(ff);
                }
            }
        }

        public void SetFont(FontFamily ff)
        {
            textFont = new Font(ff, 12f);
            using (var g = Graphics.FromImage(page1))
            {
                fTextMetricsM = g.MeasureString("mmmmmmmmmmmmmmmmmmmm", textFont);
                fTextMetricsM.Width /= 20;
            }
            iTextMetricsM = new Size((int)fTextMetricsM.Width, (int)fTextMetricsM.Height);
            //textMetricsM.Width -= 1;
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            AllocatePage();
            ResizeBuffer();
            FullScreenRefresh();
            //Print($"resize {screenBuffer.Size}");
        }

        private void ResizeBuffer()
        {
            int nrows = (int)(page1.Height / iTextMetricsM.Height);
            int ncols = (int)(page1.Width / iTextMetricsM.Width);
            if (nrows != screen.Rows || ncols != screen.Cols)
            {
                screen.Resize(nrows, ncols);
            }
        }

        private void ScreenBuffer_HandleCreated(object sender, EventArgs e)
        {
            AllocatePage();
            ResizeBuffer();
            FullScreenRefresh();
        }

        void AllocatePage()
        {
            page1 = new Bitmap(screenBuffer.Width, screenBuffer.Height, System.Drawing.Imaging.PixelFormat.Format32bppPArgb);
            using (var g = Graphics.FromImage(page1))
            {
                g.Clear(screenBackground);
            }
            SetFont("Consolas");
            screenBuffer.Image = page1;
        }


        private void DrawCursor(Graphics g)
        {
            var cpos = screen.Cursor;
            //Console.WriteLine($"Draw cursor @{cpos.r},{cpos.c}");
            Brush fgBrush = new SolidBrush(screen.ActiveFgColor);
            Pen fgPen = new Pen(fgBrush);
            Point point = new Point(cpos.c * iTextMetricsM.Width, cpos.r * iTextMetricsM.Height);
            Rectangle cell = new Rectangle(point, new Size(iTextMetricsM.Width-1, iTextMetricsM.Height-1));
            g.DrawRectangle(fgPen, cell);
        }

        public void DrawScreen(Graphics g)
        {
            foreach ((ScreenBuffer.ScreenPos pos, ScreenBuffer.Cell cell) in screen.RefreshScreen())
            {
                DrawChar(g, pos.r, pos.c, cell.ch, cell.fgColor, cell.bgColor);
            }
            DrawCursor(g);
        }

        public void DrawChar(Graphics g, int r, int c, char ch, Color _fg, Color _bg)
        {
            //Debug.Assert(ch != '\0');
            //Console.WriteLine($"Draw char @{r},{c}");
            SolidBrush fg = new SolidBrush(_fg);
            SolidBrush bg = new SolidBrush(_bg);
            Point point = new Point(c * iTextMetricsM.Width, r * iTextMetricsM.Height);
            RectangleF cell = new Rectangle(point, iTextMetricsM);
            g.FillRectangle(bg, cell);
            g.DrawString(ch.ToString(), textFont, fg, new Point(c * iTextMetricsM.Width-2, r * iTextMetricsM.Height));
        }

        public void Print(string s)
        {
            foreach (var ch in s)
            {
                screen.Print(ch);
            }
        }

        public ScreenBuffer.ScreenPos CursorPosition()
        {
            return screen.Cursor;
        }

        public void MoveCursor(int r, int c)
        {
            screen.MoveCursor(new ScreenBuffer.ScreenPos(r, c));

        }




        public void FullScreenRefresh()
        {
            using (var g = Graphics.FromImage(page1))
            {
                DrawScreen(g);
            }
            QuickRefresh();
        }

        public void QuickRefresh()
        {
            if (dirty)
            {
                screenBuffer.Invalidate();
                //screenBuffer.Image = page1;
                dirty = false;
            }
        }

        public void SetChar(ScreenBuffer.ScreenPos pos, char character)
        {
            screen.SetChar(pos, character);
        }

        //public void InitChar(ScreenPos pos)
        //{
        //    int idx = pos.r * cols + pos.c;
        //    if (txtBuffer[idx] == '\0')
        //    {
        //        SetChar(pos, ' ');
        //    }
        //}

        public void SetChar(int r, int c, char character)
        {
            SetChar(new ScreenBuffer.ScreenPos(r, c), character);
        }

        public void ClearScreen()
        {
            screen.ClearScreen();
        }

        private void screenRefreshTimer_Tick(object sender, EventArgs e)
        {
            dirty = true;
            if (dirty)
            {
                var g = Graphics.FromImage(page1);
                DrawScreen(g);
                g.Dispose();

                screenBuffer.Invalidate();
                dirty = false;
            }
        }
    }
}
