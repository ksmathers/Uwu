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
        private static Heartbeat _instance;
        public delegate void UpdateHandler(float dt);
        event UpdateHandler Update;
        long lastUpdate;
        int fps;

        public int Fps {
            get { return fps; }
            set {
                fps = value;
                int interval_ms = 1000 / fps;
                tUpdate.Interval = interval_ms;
            }
        }

        public Heartbeat()
        {
            lastUpdate = Runtime.Milliseconds;
            InitializeComponent();
            tUpdate.Tick += TUpdate_Tick;
            tUpdate.Start();
            _instance = this;
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

        public static void AddListener(UpdateHandler hdl)
        {
            if (_instance == null) throw new InvalidOperationException("can't add listener until heartbeat has been initialized");
            _instance.Update += hdl;
        }

        public static void RemoveListener(UpdateHandler hdl)
        {
            _instance.Update -= hdl;
        }



        public Heartbeat(IContainer container)
        {
            container.Add(this);

            InitializeComponent();
        }
    }
}
