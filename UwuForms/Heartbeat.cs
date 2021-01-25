using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Uwu.Core;

namespace UwuForms
{
    public partial class Heartbeat : Component
    {
        public delegate void UpdateHandler(float dt);
        public event UpdateHandler Update;
        long lastUpdate;
        int fps=0;

        /// <summary>
        /// Sets the heartbeat update rate in frames per second
        /// </summary>
        public int Fps {
            get { return fps; }
            set {
                InitFps(value);
            }
        }

        void InitFps(int newfps)
        {
            lastUpdate = Runtime.Milliseconds;
            fps = newfps;
            if (fps == 0) {
                tUpdate.Stop();
            } else {
                int interval_ms = 1000 / fps;
                tUpdate.Interval = interval_ms;
                tUpdate.Start();
            }
        }

        /// <summary>
        /// A heartbeat component is required to tie together any canvases or other animated controls that
        /// are updated on a frame interval.
        /// </summary>
        public Heartbeat()
        {
            InitializeComponent();
        }


        private void TUpdate_Tick(object sender, EventArgs e)
        {
            long now = Runtime.Milliseconds;
            OnUpdate((now-lastUpdate)/1000f);
            lastUpdate = now;
        }

        protected void OnUpdate(float dt)
        {
            Update?.Invoke(dt);
        }

        /// <summary>
        /// A heartbeat component is required to tie together any canvases or other animated controls that
        /// are updated on a frame interval.
        /// </summary>
        /// <param name="container">parent component container</param>
        public Heartbeat(IContainer container)
        {
            container.Add(this);
            InitializeComponent();
        }
    }
}
