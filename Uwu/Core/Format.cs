using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Uwu.Core
{
    /// <summary>
    /// A unified formatting class for the base variable types.  This libary is intended for use
    /// by the ConfigIni class to convert native types into values that can be saved back into an 
    /// INI file.   
    /// </summary>
    public class Format
    {
        // TODO: Notably missing is a string encoder.

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
