using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Uwu.Core;

namespace UwuTest
{
    [TestClass]
    public class UwuInterpolate
    {

        Dictionary<string, string> vars = new Dictionary<string, string>() { 
            { "DEBUG", "True" },
            { "PYTHON", @"%HOME%\anaconda3\bin\pythonw.exe" },
            { "HOME", @"C:\Users\Kevin" },
            { "OperatingSystem", @"Windows" },
            { "__PROTO__", @"TCP" },
            { "VNC4Windows", @"TightVNC" }
        };

        [DataRow("%DEBUG%", "True")]
        [DataRow("%PYTHON%", @"C:\Users\Kevin\anaconda3\bin\pythonw.exe")]
        [DataRow("Tempus fugit, memento %OperatingSystem%!", "Tempus fugit, memento Windows!")]
        [DataRow("%__PROTO__%", "TCP")]
        [DataRow("%UNDEFINED%", "%UNDEFINED%")]
        [DataRow("%VNC4%OperatingSystem%%", "TightVNC")]
        [TestMethod]
        public void TestInterpolate(string sval, string result)
        {
            Assert.AreEqual(result, sval.Interpolate(vars));
        }
    }
}
