using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Imaging;
using Uwu.Core;

namespace UwuForms
{
    public partial class Canvas : UserControl
    {
        Bitmap frame1;
        Bitmap frame2;
        Bitmap[] frames;
        int frameNo = 0;
        int ballSize = 80;

        bool debug = true;
        int width = 1024;
        int height = 1024;

        /// <summary>
        /// Double buffered drawing canvas control with a built in timer for drawing updates.   Use the
        /// DrawCanvas event to add your own drawing code.   Call Start() to begin the clock.
        /// </summary>
        public Canvas()
        {
            UpdateFrameBuffer();
            InitializeComponent();

        }

        /// <summary>
        /// Starts the update clock.  
        /// </summary>
        /// <param name="fps">update rate in frames per second</param>
        public void Start(int fps=10)
        {
            SetFrameRate(fps);
            UpdateCanvas(0);
            LastUpdateMsec = Runtime.Milliseconds;
            Heartbeat.AddListener(UpdateCanvas);
        }

        public int CanvasWidth {
            get { return width; }
            set { width = value; UpdateFrameBuffer(); }
        }

        public int CanvasHeight {
            get { return height; }
            set { height = value; UpdateFrameBuffer(); }
        }

        public Size CanvasSize {
            get { return new Size(width, height); }
            set { 
                width = value.Width;
                height = value.Height;
                UpdateFrameBuffer();
            }
        }

        void UpdateFrameBuffer()
        {
            frame1 = new Bitmap(width, height, PixelFormat.Format32bppPArgb);
            frame2 = new Bitmap(width, height, PixelFormat.Format32bppPArgb);
            frames = new Bitmap[] { frame1, frame2 };
        }

        public void SetFrameRate(int fps)
        {
            int interval = 1000 / fps;
            if (interval < 1) interval = 1;
            if (interval > 1000) interval = 1000;
            timer1.Interval = interval;
        }

        long LastUpdateMsec;
        public delegate void DrawCanvasHandler(object sender, Graphics g, float dt);
        public event DrawCanvasHandler DrawCanvas;

        public void OnDrawCanvas(Graphics g)
        {
            int msec = Uwu.Core.Runtime.Milliseconds;
            float dt = (msec - LastUpdateMsec) / 1000f;
            DrawCanvas?.Invoke(this, g, dt);
            LastUpdateMsec = msec;
        }

        public void UpdateCanvas(float dt)
        {
            var f = frames[frameNo];
            var g = Graphics.FromImage(f);
            var sz = new SizeF(ballSize, ballSize);
            var center = ballSize / 2;

            g.Clear(Color.Black);
            OnDrawCanvas(g);
            g.Dispose();
            pictureBox1.Image = f;
            frameNo = (frameNo + 1) % 2;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            UpdateCanvas(0);
        }
    }
}
