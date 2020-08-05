using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UwuHub
{
    public class ConfigUwuHub : Uwu.Core.ConfigIni
    {
        public ConfigUwuHub() :
            base("UwuHub", System.Environment.MachineName, "")
        { }
    }
}
