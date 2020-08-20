using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using UwuNet;

namespace UwuForms
{
    public partial class MessageDeliveryAgent : Component
    {
        public event GroupMessageHandler MessageReceived;
        public event StateChangedHandler StateChanged;

        public MessageDeliveryAgent()
        {
            InitializeComponent();
        }

        public MessageDeliveryAgent(IContainer container)
        {
            container.Add(this);
            InitializeComponent();
        }

        public IMessaging Connect(Registry registry, string connectTo)
        {
            string proto, args;
            (proto, args) = registry.ParseConnectionString(connectTo);
            var msgs = registry.Create(proto);
            msgs.MessageReceived += Msgs_MessageReceived;
            msgs.StateChanged += Msgs_StateChanged;
            msgs.Connect(args);
            return msgs;
        }

        private void Msgs_StateChanged(object sender, string newstate)
        {
            Control c = this.Container as Control;
            if (c != null && c.InvokeRequired) {
                c.BeginInvoke(new StateChangedHandler(Msgs_StateChanged), sender, newstate);
                return;
            }
            StateChanged?.Invoke(sender, newstate);
        }

        private void Msgs_MessageReceived(string msg, Options opts)
        {
            Control c = this.Container as Control;
            if (c != null && c.InvokeRequired) {
                c.BeginInvoke(new GroupMessageHandler(Msgs_MessageReceived), msg, opts);
                return;
            }
            MessageReceived?.Invoke(msg, opts);
        }
    }
}
