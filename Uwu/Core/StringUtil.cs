using System;
using System.Collections.Generic;
using System.Text;

namespace Uwu.Core
{
    public static class StringUtil
    {
        /// <summary>
        /// Removes any trailing newline characters if they is present at the end of a string
        /// </summary>
        /// <param name="istr">the string to be trimmed</param>
        /// <returns></returns>
        public static string TrimCRLF(this string istr)
        {
            while (istr.EndsWith("\n") || istr.EndsWith("\r")) {
                istr = istr.Substring(0, istr.Length - 1);
            }
            return istr;
        }

        public static string TrimEnd(this string istr, string trim)
        {
            if (istr.EndsWith(trim)) {
                istr = istr.Substring(0, istr.Length - trim.Length);
            }
            return istr;
        }

        /// <summary>
        /// Removes the outermost matching set of quote marks around a string.   Quote marks can be 
        /// single quote ('), double quote ("), triple single quote ('''), or triple double quote (""").
        /// </summary>
        /// <param name="istr">the string to be trimmed</param>
        /// <returns></returns>
        public static string Dequote(this string istr)
        {
            string local = istr;
            if (RemoveBoundary(ref local, "\"\"\"")) return local;
            if (RemoveBoundary(ref local, "'''")) return local;
            if (RemoveBoundary(ref local, "\"")) return local;
            if (RemoveBoundary(ref local, "'")) return local;
            return local;
        }

        private static bool RemoveBoundary(ref string istr, string boundary)
        {
            
            if (istr.StartsWith(boundary) && istr.EndsWith(boundary)) {
                int len = boundary.Length;
                if (istr.Length >= 2*len) istr = istr.Substring(len, istr.Length - 2*len);
                return true;
            }
            return false;
        }

        public enum EscapeHint
        {
            None,
            Quoted,
            DoubleQuoted,
            Escaped,
            TripleQuoted,
            TripleDoubleQuoted
        }

        public static string ShellEscape(this string istr, EscapeHint hint)
        {
            switch (hint) {
                case EscapeHint.DoubleQuoted:
                    istr.Replace(@"\", @"\\");
                    istr.Replace("\"", @"\""");
                    istr.Replace("\n", @"\n");
                    return $"\"{istr}\"";
                case EscapeHint.Quoted:
                    istr.Replace(@"\", @"\\");
                    istr.Replace("'", @"\'");
                    istr.Replace("\n", @"\n");
                    return $"'{istr}'";
                case EscapeHint.TripleQuoted:
                    istr.Replace(@"\", @"\\");
                    return $"'''\n{istr}'''";
                case EscapeHint.TripleDoubleQuoted:
                    istr.Replace(@"\", @"\\");
                    return $"\"\"\"\n{istr}\"\"\"";
                case EscapeHint.Escaped:
                    istr.Replace(@"\", @"\\");
                    istr.Replace(" ", @"\ ");
                    istr.Replace("\n", @"\n");
                    istr.Replace("\t", @"\t");
                    return istr;
                case EscapeHint.None:
                default:
                    return istr;
            }
        }
    }
}
