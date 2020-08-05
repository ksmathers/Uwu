using System;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UwuNet;

namespace UwuTest
{
    [TestClass]
    public class UwuMcast
    {
        volatile static int receivedCount = 0;
        volatile static int errorCount = 0;
        [TestMethod]
        public void TestMcast()
        {
            var registry = new UwuNet.Registry();
            var msgs = registry.Create("mcast");     
            msgs.MessageReceived += Msgs_MessageReceived;
            msgs.Connect();

            int sendCount = 100;
            for (int i = 0; i < sendCount; i++) { 
                Thread.Sleep(10);
                msgs.SendMessage("this is a test");
            }

            int timeout = 10;
            while (receivedCount < sendCount) {
                if (timeout-- < 0) break;
                Thread.Sleep(1000);
            }
            msgs.Stop();
            Assert.AreEqual(0, errorCount);
            Assert.AreEqual(sendCount, receivedCount);      
        }

        private void Msgs_MessageReceived(string msg, Options opts)
        {
            if (msg == "this is a test") receivedCount++;
            else errorCount++;
        }
    }
}
