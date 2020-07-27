using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Text;
using Uwu.Config;
using Uwu.Core;
using System.Collections.Generic;

namespace UwuTest
{
    [TestClass]
    public class UwuIniData
    {

        [TestMethod]
        public void TestUwuIniData()
        {
            string iniDataString = @"
[default]
connectTo=dbus:auto
randomSeed=42
saveDir=%DOCUMENTS%
fileOpenDir=%DESKTOP%
welcome message=""""""
Welcome to UwuIniData
Your INI is in safe hands
""""""
login_screen = Username:\
Password:

[accessCode]
ident='1234'
secret='''
abcdefghijklmnop'''

[ZEUS]
connectTo=dbus:chiron.ank.com

[CHIRON]
connectTo=dbus:server
";
            var idata = new IniData(); 
            var vars = new Dictionary<string, string>();

            idata.LoadString(iniDataString);
            Assert.AreEqual("dbus:server", idata.GetParam("CHIRON", "connectTo"));
            Assert.AreEqual(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments), idata.GetParam("default", "saveDir").Interpolate(vars));
            Assert.AreEqual("%DESKTOP%", idata.GetParam("default", "fileOpenDir"));
            Assert.AreEqual("Welcome to UwuIniData\nYour INI is in safe hands\n", idata.GetParam("default", "welcome message"));
            Assert.AreEqual("1234", idata.GetParam("accessCode", "ident"));
            Assert.AreEqual("abcdefghijklmnop", idata.GetParam("accessCode", "secret"));
            Assert.AreEqual("unknown", idata.GetParam("default", "unknownkey", "unknown"));
            Assert.AreEqual(42, Parse.Int(idata.GetParam("default", "randomSeed"), 0));
            Assert.AreEqual("Username:\nPassword:", idata.GetParam("default", "login_screen"));
        }
    }
}
