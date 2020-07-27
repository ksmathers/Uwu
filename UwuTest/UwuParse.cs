using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Uwu.Core;

namespace UwuTest
{
    [TestClass]
    public class UwuParse
    {
        [DataRow("123", 0, 123)]
        [DataRow("-123", 0, -123)]
        [DataRow("abc", 42, 42)]
        [DataRow("42c", 0, 0)]
        [DataRow("42.4", 0, 0)]
        [TestMethod]
        public void TestParseInt(string sval, int dfl, int result)
        {
            Assert.AreEqual(Parse.Int(sval, dfl), result);
        }

        [DataRow("123", 0f, 123f)]
        [DataRow("-123", 0f, -123f)]
        [DataRow("abc", 42.5f, 42.5f)]
        [DataRow("42c", 0f, 0f)]
        [DataRow("1.25", 0f, 1.25f)]
        [DataRow("-6.177e32", 0f, -6.177e32f)]
        [DataRow("2.414e-12", 0f, 2.414e-12f)]
        [TestMethod]
        public void TestParseFloat(string sval, float dfl, float result)
        {
            Assert.AreEqual(Parse.Float(sval, dfl), result);
        }

        [DataRow("123", 0, 123)]
        [DataRow("-123", 0f, -123)]
        [DataRow("abc", 42.5, 42.5)]
        [DataRow("42c", 0, 0)]
        [DataRow("1.25", 0, 1.25)]
        [DataRow("-6.177e32", 0, -6.177e32)]
        [DataRow("2.414e-12", 0, 2.414e-12)]
        [DataRow("Infinity", 0, Double.PositiveInfinity)]
        [DataRow("-Infinity", 0, Double.NegativeInfinity)]
        [DataRow("NaN", 0, Double.NaN)]
        [DataRow("1.7e308", 0, 1.7e308)]
        [TestMethod]
        public void TestParseDouble(string sval, double dfl, double result)
        {
            Assert.AreEqual(result, Parse.Double(sval, dfl));
        }

        public enum DayOfWeek { Mon, Tue, Wed, Thu, Fri, Sat, Sun };
        public enum Month { Jan, Feb, Mar, Apr, May, Jun, Jul, Aug, Sep, Oct, Nov, Dec }

        [DataRow("Mon", DayOfWeek.Mon)]
        [DataRow("Tue", DayOfWeek.Tue)]
        [DataRow("Sat", DayOfWeek.Sat)]
        [DataRow("Jan", DayOfWeek.Mon)]
        [TestMethod]
        public void TestParseEnumDOW(string sval, DayOfWeek result)
        {
            Assert.AreEqual(result, Parse.Enum<DayOfWeek>(sval));
        }

        [DataRow("Dec", Month.Dec)]
        [DataRow("Feb", Month.Feb)]
        [DataRow("Jul", Month.Jul)]
        [DataRow("Oct ", Month.Oct)]
        [DataRow(" Nov", Month.Nov)]
        [DataRow("/May/", Month.Jan)]
        [DataRow(null, Month.Jan)]
        [TestMethod]
        public void TestParseEnumMonth(string sval, Month result)
        {
            Assert.AreEqual(result, Parse.Enum<Month>(sval));
        }

        [DataRow("on", false, true)]
        [DataRow("True", false, true)]
        [DataRow("YES", false, true)]
        [DataRow("no", true, false)]
        [DataRow("x", true, true)]
        [DataRow("x", false, false)]
        [TestMethod]
        public void TestParseBool(string sval, bool defaultVal, bool result)
        {
            Assert.AreEqual(result, Parse.Bool(sval, defaultVal));
        }
    }
}
