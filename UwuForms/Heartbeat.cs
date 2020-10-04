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

        public Heartbeat(IContainer container)
        {
            container.Add(this);
            InitializeComponent();
        }
    }
}
