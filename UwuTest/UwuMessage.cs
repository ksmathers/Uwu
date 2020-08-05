using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UwuNet;

namespace UwuTest
{
    [TestClass]
    public class UwuMessage
    {
        [TestMethod]
        public void TestMessage()
        {
            var iobuf = new IOBuffer();
            string testbody = "this is a test";
            var txmsg = new OrchestrationMessage(testbody, Options.LAN);
            Message.Transmit(iobuf, txmsg);

            int SYNC = 4;
            int MLEN = 4;
            int MTYP = 4;
            int OPTS = 4;
            int TERM = 1;
            Assert.AreEqual(SYNC + MLEN + MTYP + OPTS + testbody.Length + TERM, iobuf.Length);
            var rxmsg = Message.Receive(iobuf) as OrchestrationMessage;
            Assert.AreEqual(txmsg.Body, rxmsg.Body);
            Assert.AreEqual(txmsg.IsLan, rxmsg.IsLan);
            Assert.AreEqual(txmsg.IsWan, rxmsg.IsWan);
        }
    }
}
