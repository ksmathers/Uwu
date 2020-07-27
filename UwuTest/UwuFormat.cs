using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Uwu.Core;

namespace UwuTest
{
    [TestClass]
    public class UwuFormat
    {
        public enum Month { Jan, Feb, Mar, Apr, May, Jun, Jul, Aug, Sep, Oct, Nov, Dec }

        [DataRow(Month.Dec, "Dec")]
        [DataRow(Month.Feb, "Feb")]
        [DataRow(Month.Jul, "Jul")]
        [DataRow(Month.Oct, "Oct")]
        [DataRow(Month.Nov, "Nov")]
        [DataRow(Month.Jan, "Jan")]
        [TestMethod]
        public void TestFormatEnumMonth(Month val, string result)
        {
            Assert.AreEqual(Format.Enum<Month>(val), result);
        }

        [DataRow(123, 10, "123")]
        [DataRow(-123, 10, "-123")]
        [DataRow(0x42, 16, "42")]
        [DataRow(8, 8, "10")]
        [DataRow(10, 2, "1010")]
        [TestMethod]
        public void TestFormatInt(int val, int radix, string result)
        {
            Assert.AreEqual(Format.Int(val, radix), result);
        }

        [DataRow(15.5, "15.5")]
        [DataRow(12e3, "12000")]
        [DataRow(1.5e32, "1.5E+32")]
        [DataRow(Double.NaN, "NaN")]
        [DataRow(Double.PositiveInfinity, "Infinity")]
        [DataRow(Double.NegativeInfinity, "-Infinity")]
        [TestMethod]
        public void TestFormatDouble(double val, string result)
        {
            Assert.AreEqual(Format.Double(val), result);
        }
    }
}
