using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;

namespace UwuForms
{
    public class Keymap
    {
        Dictionary<Keys, string> printable = new Dictionary<Keys, string>()
        {
            { Keys.A, "aA" },
            { Keys.B, "bB" },
            { Keys.C, "cC" },
            { Keys.D, "dD" },
            { Keys.E, "eE" },
            { Keys.F, "fF" },
            { Keys.G, "gG" },
            { Keys.H, "hH" },
            { Keys.I, "iI" },
            { Keys.J, "jJ" },
            { Keys.K, "kK" },
            { Keys.L, "lL" },
            { Keys.M, "mM" },
            { Keys.N, "nN" },
            { Keys.O, "oO" },
            { Keys.P, "pP" },
            { Keys.Q, "qQ" },
            { Keys.R, "rR" },
            { Keys.S, "sS" },
            { Keys.T, "tT" },
            { Keys.U, "uU" },
            { Keys.V, "vV" },
            { Keys.W, "wW" },
            { Keys.X, "xX" },
            { Keys.Y, "yY" },
            { Keys.Z, "zZ" },
            { Keys.D0, "0)" },
            { Keys.D1, "1!" },
            { Keys.D2, "2@" },
            { Keys.D3, "3#" },
            { Keys.D4, "4$" },
            { Keys.D5, "5%" },
            { Keys.D6, "6^" },
            { Keys.D7, "7&" },
            { Keys.D8, "8*" },
            { Keys.D9, "9(" },
            { Keys.NumPad0, "00" },
            { Keys.NumPad1, "11" },
            { Keys.NumPad2, "22" },
            { Keys.NumPad3, "33" },
            { Keys.NumPad4, "44" },
            { Keys.NumPad5, "55" },
            { Keys.NumPad6, "66" },
            { Keys.NumPad7, "77" },
            { Keys.NumPad8, "88" },
            { Keys.NumPad9, "99" },
            { Keys.Divide, "//" },
            { Keys.Multiply, "**" },
            { Keys.Decimal, ".." },
            { Keys.Add, "++" },
            { Keys.Subtract, "--" },
            { Keys.Oemcomma, ",<" },
            { Keys.OemPeriod, ".>" },
            { Keys.OemQuestion, "/?" },
            { Keys.OemOpenBrackets, "[{" },
            { Keys.OemCloseBrackets, "]}" },
            { Keys.OemSemicolon, ";:" },
            { Keys.OemQuotes, "'\"" },
            { Keys.OemPipe, "\\|" },
            { Keys.Oemplus, "=+" },
            { Keys.OemMinus, "-_" },
            { Keys.OemBackslash, "\\|" },
            { Keys.Oemtilde, "`~" },
            { Keys.Space, "  " }
        };

        public bool isPrintable(Keys k)
        {
            if (printable.ContainsKey(k))
            {
                return true;
            }
            return false;
        }

        public bool isControlCode(Keys k)
        {
            return false;
        }

        public char toChar(Keys k, bool shift)
        {
            if (isPrintable(k))
            {
                return printable[k][shift ? 1 : 0];
            }
            throw new ArgumentException();
        }
    }
}
