using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Uwu.Core
{
    public class Parse
    {
        public static int Int(string sval, int defaultVal=0)
        {
            bool ok = int.TryParse(sval, out int val);
            if (!ok) val = defaultVal;
            return val;
        }

        public static float Float(string sval, float defaultVal=0)
        {
            bool ok = float.TryParse(sval, NumberStyles.Float, CultureInfo.InvariantCulture, out float val);
            if (!ok) val = defaultVal;
            return val;
        }

        public static double Double(string sval, double defaultVal = 0)
        {
            bool ok = double.TryParse(sval, NumberStyles.Float, CultureInfo.InvariantCulture, out double val);
            if (!ok) val = defaultVal;
            return val;
        }

        public static T Enum<T>(string sval, T defaultVal) where T : struct, IConvertible
        {
            T val;
            bool ok = System.Enum.TryParse<T>(sval, out val);
            if (!ok) { val = defaultVal; }
            return val;
        }

        public static T Enum<T>(string sval) where T : struct, IConvertible
        {
            T defaultVal = new T();
            return Parse.Enum<T>(sval, defaultVal);
        }

        public static DateTime Date(string sval, DateTime defaultVal)
        {
            var ok = DateTime.TryParse(sval, CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out DateTime result);
            if (!ok) {
                result = defaultVal;
            }
            return defaultVal;
        }

        public static DateTime Date(string sval)
        {
            return Parse.Date(sval, new DateTime(1970,01,01));
        }

        public static bool Bool(string sval)
        {
            bool result = Parse.Bool(sval, false);
            return result;
        }

        public static bool Bool(string sval, bool defaultVal)
        {
            bool on = false;
            on |= string.Compare(sval, "true", ignoreCase: true) == 0;
            on |= string.Compare(sval, "yes", ignoreCase: true) == 0;
            on |= string.Compare(sval, "on", ignoreCase: true) == 0;

            bool off = false;
            off |= string.Compare(sval, "false", ignoreCase: true) == 0;
            off |= string.Compare(sval, "no", ignoreCase: true) == 0;
            off |= string.Compare(sval, "off", ignoreCase: true) == 0;

            bool result = defaultVal;
            if (on) result = true;
            if (off) result = false;
            return result;
        }

    }
}
