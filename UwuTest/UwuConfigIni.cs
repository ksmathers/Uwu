using System;
using System.ComponentModel.Design;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Uwu.Core;

class AppConfig : ConfigIni
{
    const string resource = "CHIRON";
    const string defaultConfig = @"
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

[menu]
1 = open
2 = quit

[ZEUS]
connectTo=dbus:chiron.ank.com

[CHIRON]
connectTo=dbus:server
";

    public AppConfig() :
        base("UwuTest", resource, defaultConfig) { }

    public int RandomSeed { get { return Parse.Int(ini.GetDefault("randomSeed"), 0); } }
    public string SaveDir { get { return ini.Interpolate(ini.GetDefault("saveDir")); } }
    public string Ident { get { return ini.Data.GetParam("accessCode", "ident"); } }
    public string Secret { get { return ini.Data.GetParam("accessCode", "secret"); } }

    public string LoginScreen { get { return ini.GetDefault("login_screen"); } }
    public string WelcomeMessage { get { return ini.GetDefault("welcome message"); } }
}


namespace UwuTest
{
    [TestClass]
    public class UwuConfigIni
    {
        [TestMethod]
        public void TestConfig()
        {
            var cfg = new AppConfig();
            Assert.AreEqual("dbus:server", cfg.ConnectTo);
            Assert.AreEqual(42, cfg.RandomSeed);
            Assert.AreEqual("1234", cfg.Ident);
            string[] menuitems = new string[] { "1", "2" };
            int i = 0;
            foreach (var key in cfg.SectionKeys("menu")) {
                Assert.AreEqual(menuitems[i++], key);
            }
            Assert.AreEqual(2, i);
        }

        [TestMethod]
        public void TestSave()
        {
            var path = System.IO.Path.Combine(
                System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData),
                "UwuTest.ini");
            if (System.IO.File.Exists(path)) System.IO.File.Delete(path);
            Assert.IsFalse(System.IO.File.Exists(path));
            var cfg = new AppConfig();
            cfg.Save();
            Assert.IsTrue(System.IO.File.Exists(path));
            var fp = System.IO.File.OpenRead(path);
            Assert.IsTrue(fp.Length > 100);
        }
    }
}
