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
        bool autoSize = true;

        /// <summary>
        /// When enabled (default) the painting surface of the canvas tracks the size of the control.   When disabled the painting surface can be set to a fixed resolution (default 1024x1024) and
        /// is scaled to fit the control.
        /// </summary>
        public bool CanvasAutoSize {
            get { return autoSize; }
            set { autoSize = value; }
        }

        /// <summary>
        /// Double buffered drawing canvas control with a built in timer for drawing updates.   Use the
        /// DrawCanvas event to add your own drawing code.   Call Start() to begin the clock.
        /// </summary>
        public Canvas()
        {
            UpdateFrameBuffer();
            InitializeComponent();
            pictureBox1.MouseDown += PictureBox1_MouseDown;
            pictureBox1.MouseUp += PictureBox1_MouseUp;
            pictureBox1.MouseClick += PictureBox1_MouseClick;
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            if (autoSize) CanvasSize = this.Size;
        }

        private void PictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            OnMouseClick(e);
        }

        private void PictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            OnMouseUp(e);
        }

        private void PictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            OnMouseDown(e);
        }

        /// <summary>
        /// Starts updating the canvas on a periodic basis.   
        /// </summary>
        /// <param name="hr">heartbeat clock</param>
        public void Start(Heartbeat hr)
        {
            UpdateCanvas(0);
            LastUpdateMsec = Runtime.Milliseconds;
            hr.Update += UpdateCanvas;
        }

        /// <summary>
        /// Changes the width of the canvas frame buffer.  Defaults to 1024.
        /// </summary>
        public int CanvasWidth {
            get { return width; }
            set { width = value; UpdateFrameBuffer(); }
        }

        /// <summary>
        /// Changes the height of hte canvas frame buffer.  Defaults to 1024
        /// </summary>
        public int CanvasHeight {
            get { return height; }
            set { height = value; UpdateFrameBuffer(); }
        }

        /// <summary>
        /// Changes the size of the canvas frame buffer.   Defaults to 1024x1024.
        /// </summary>
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

        long LastUpdateMsec;
        public delegate void DrawCanvasHandler(object sender, Graphics g, float dt);
        public event DrawCanvasHandler DrawCanvas;

        /// <summary>
        /// Activates the DrawCanvas event
        /// </summary>
        /// <param name="g">Graphics2D attached to the inactive buffer</param>
        public void OnDrawCanvas(Graphics g)
        {
            int msec = Uwu.Core.Runtime.Milliseconds;
            float dt = (msec - LastUpdateMsec) / 1000f;
            DrawCanvas?.Invoke(this, g, dt);
            LastUpdateMsec = msec;
        }

        /// <summary>
        /// Activates the DrawCanvas to fill the back buffer and then flips buffers
        /// </summary>
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

    }
}
