using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;

namespace Uwu.Core
{
    public static class InterpolateString
    {
        /// <summary>
        /// Interpolates strings with variables of the form "%Variable_Name%".  Variables are selected
        /// first by looking in the supplied dictionary 'dict', and failing that from the global application 
        /// Environment instance.  See the 'Environment' class.
        /// 
        /// Any variables which are missing from both 'dict' and the environment are left unchanged.
        /// </summary>
        /// <param name="sval">String to be interpolated</param>
        /// <param name="dict">Dictionary of variable names to substitute</param>
        /// <returns></returns>
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
