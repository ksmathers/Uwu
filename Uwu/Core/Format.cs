using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Uwu.Core
{
    public class Format
    {
        public static string Int(int val, int ibase=10)
        {
            return Convert.ToString(val, ibase);
        }

        public static string Double(double val)
        {
            return Convert.ToString(val, CultureInfo.InvariantCulture);
        }

        public static string Float(float val)
        {
            return Convert.ToString(val, CultureInfo.InvariantCulture);
        }

        public static string Enum<T>(T val) where T : struct, IConvertible
        {
            return Convert.ToString(val);
        }
    }
}
