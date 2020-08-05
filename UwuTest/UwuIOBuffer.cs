using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UwuNet;

namespace UwuTest
{
    [TestClass]
    public class UwuIOBuffer
    {
        Random rnd;
        IOBuffer iobuf;
        [TestMethod]
        public void RandomInsertTest()
        {
            iobuf = new IOBuffer();
            rnd = new Random(625232);
            byte[] buf = new byte[100];
            for (int i = 0; i < buf.Length; i++) buf[i] = (byte)i;
            for (int i = 0; i < 500; i++) {
                int ll = rnd.Next(0, 100);
            }
        }
    }
}
