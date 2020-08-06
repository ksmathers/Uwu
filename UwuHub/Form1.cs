using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using UwuNet;

namespace UwuHub
{
    public partial class Form1 : Form
    {
        IMessaging msgs;
        Registry registry;
        ConfigUwuHub cfg;

        public Form1()
        {
            cfg = new ConfigUwuHub();
            registry = new Registry();
            InitializeComponent();
            Application.ApplicationExit += Application_ApplicationExit;
        }

        private void Application_ApplicationExit(object sender, EventArgs e)
        {
            if (msgs != null) msgs.Stop();
        }

        private void quitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void connectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var cxn = new FormConnectTo();
            cxn.ConnectTo = "mcast:2";
            if (cxn.ShowDialog() == DialogResult.OK) {
                string proto, args;
                (proto, args) = registry.ParseConnectionString(cxn.ConnectTo);
                msgs = registry.Create(proto);
                msgs.MessageReceived += Msgs_MessageReceived;
                msgs.StateChanged += Msgs_StateChanged;
                msgs.Connect(args);
            }
        }

        private void Msgs_StateChanged(object sender, string newstate)
        {
            if (this.InvokeRequired) {
                this.BeginInvoke(new StateChangedHandler(Msgs_StateChanged), sender, newstate);
                return;
            }
            tsslConnectionState.Text = newstate;
        }

        List<string> lines = new List<string>();

        public void Print(string msg)
        {
            lines.Add(msg);
            if (lines.Count > 1000) lines = lines.Skip(10).ToList();
            StringBuilder sb = new StringBuilder();
            foreach (var line in lines) {
                sb.AppendLine(line);
            }
            tbScroller.Text = sb.ToString();
            tbScroller.ScrollToCaret();
        }

        private void Msgs_MessageReceived(string msg, Options opts)
        {
            if (this.InvokeRequired) {
                this.BeginInvoke(new OrchestrationMessageHandler(Msgs_MessageReceived), msg, opts);
                return;
            }
            Print($"> {msg}");
        }

        private void tbCommand_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) {
                if (msgs == null) {
                    MessageBox.Show("Not connected");
                    return;
                }
                e.Handled = true;
                var cmd = tbCommand.Text;
                tbCommand.Text = "";
                Print($"+ {cmd}");
                msgs.SendMessage(cmd);
            }
        }
    }
}
