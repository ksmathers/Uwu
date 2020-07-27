using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Uwu.Core;

namespace UwuTest
{
    [TestClass]
    public class UwuArglist
    {
        [TestMethod]
        public void TestArglistParsing()
        {
            Arglist args = new Arglist(@"test C:\\Users\\Kevin\\Profile");
            Assert.AreEqual(@"test", args.Shift());
            Assert.AreEqual(@"C:\Users\Kevin\Profile", args.Shift());
        }

        [TestMethod]
        public void TestArglistParsingDQ()
        {
            Arglist args = new Arglist("\"Don't try to stop me!\" SIGTERM");
            Assert.AreEqual(@"Don't try to stop me!", args.Shift());
            Assert.AreEqual(@"SIGTERM", args.Shift());
        }

        [TestMethod]
        public void TestArglistParsingSQ()
        {
            Arglist args = new Arglist("'echo $DISPLAY >>/dev/null' || die");
            Assert.AreEqual(@"echo $DISPLAY >>/dev/null", args.Shift());
            Assert.AreEqual(@"||", args.Shift());
            Assert.AreEqual(@"die", args.Shift());
        }

        [TestMethod]
        public void TestArglistEscapeNL()
        {
            Arglist args = new Arglist("send 'ls -ltr\n'");
            Assert.AreEqual(@"send", args.Shift());
            Assert.AreEqual("ls -ltr\n", args.Shift());
        }

        [TestMethod]
        public void TestArglistEscapeSpace()
        {
            Arglist args = new Arglist(@"open C:\\Program\ Files\ (x86)\\UwuCore\\setup.ini");
            Assert.AreEqual(@"open", args.Shift());
            Assert.AreEqual(@"C:\Program Files (x86)\UwuCore\setup.ini", args.Shift());
        }

        [TestMethod]
        public void TestArglistQuotedInternal()
        {
            Arglist args = new Arglist("'my name'='Joe Bloggs'");
            Assert.AreEqual(@"my name=Joe Bloggs", args.Shift());
        }

        [TestMethod]
        public void TestArglistEscapeUnicode()
        {
            Arglist args = new Arglist(@"alpha U+03b1 \u03b1");
            Assert.AreEqual(@"alpha", args.Shift());
            Assert.AreEqual(@"U+03b1", args.Shift());
            Assert.AreEqual("\u03b1", args.Shift());
        }

        [TestMethod]
        public void TestArglistJoin()
        {
            Arglist args = new Arglist("one 't w o' \"ain't that you?\"");
            Assert.AreEqual("one \"t w o\" \"ain't that you?\"", args.Join());
        }
    }
}
