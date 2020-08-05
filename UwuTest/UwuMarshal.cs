using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UwuNet;

namespace UwuTest
{
    [TestClass]
    public class UwuMarshal
    {
        [TestMethod]
        public void TestMarshalInt32()
        {
            var marshal = new Marshal();
            marshal.WriteInt32(0x12345678);
            var iobuf = marshal.IOBuf;
            Assert.AreEqual(0x78, iobuf.Peek(0));
            Assert.AreEqual(0x56, iobuf.Peek(1));
            Assert.AreEqual(0x34, iobuf.Peek(2));
            Assert.AreEqual(0x12, iobuf.Peek(3));
            Assert.AreEqual(4, iobuf.Length);

            marshal.WriteInt32(-1);
            Assert.AreEqual(0x12345678, marshal.ReadInt32());
            Assert.AreEqual(-1, marshal.ReadInt32());

        }

        [DataRow("This is a test")]
        [DataRow("可愛すぎる")]
        [DataRow(@"😀⭠Extended character block (Emoji)")]
        [TestMethod]
        public void TestMarshalString(string sval)
        {
            var marshal = new Marshal();
            marshal.WriteString(sval);
            Assert.AreEqual(sval, marshal.ReadString());
        }
    }
}
