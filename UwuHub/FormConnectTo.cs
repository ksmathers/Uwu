using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UwuHub
{
    public partial class FormConnectTo : Form
    {
        public FormConnectTo()
        {
            InitializeComponent();
        }

        public string ConnectTo {
            get { return tbConnectTo.Text; }
            set { tbConnectTo.Text = value; }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
