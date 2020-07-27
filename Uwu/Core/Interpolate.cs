using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;

namespace Uwu.Core
{
    public static class InterpolateString
    {
        public static String Interpolate(this string sval, Dictionary<string, string> dict) 
        {
            Regex re = new Regex(@"%([A-Za-z_][A-Za-z0-9_]*)%");
            int cpos = 0;
            var m = re.Match(sval, cpos);
            while (m.Success) {
                Debug.WriteLine($"sval -> {sval}");
                var key = m.Groups[1].Value;
                if (dict.ContainsKey(key)) {
                    sval = re.Replace(sval, dict[key], 1, cpos);
                    cpos = 0;
                } else {
                    string value = Environment.Instance.GetVar(key);
                    if (value != null) {
                        sval = re.Replace(sval, value, 1, cpos);
                        
                        cpos = 0;
                    } else {
                        cpos = m.Index + 1;
                    }
                }
                m = re.Match(sval, cpos);
            }
            return sval;
        }
    }
}
