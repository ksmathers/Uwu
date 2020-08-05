using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UwuNet;

namespace UwuTest
{
    [TestClass]
    public class UwuIOBuffer
    {
        [TestMethod]
        public void RandomInsertTest()
        {
            IOBuffer iobuf = new IOBuffer();
            Random rnd = new Random(625232);
            byte[] buf = new byte[100];
            for (int i = 0; i < buf.Length; i++) buf[i] = (byte)i;
            for (int i = 0; i < 500; i++) {
                int ll = rnd.Next(0, 100);
                iobuf.Write(buf.Take(ll));
            }

            rnd = new Random(625232);
            for (int i = 0; i < 500; i++) {
                int ll = rnd.Next(0, 100);
                var buf2 = iobuf.Read(ll);
                for (int i2 = 0; i2 < buf2.Length; i2++) {
                    Assert.AreEqual(i2, buf2[i2]);
                }
                iobuf.Compress();
            }

            Assert.AreEqual(0, iobuf.Length);
        }
    }
}
