using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace Uwu.Core
{
    public class Arglist
    {
        enum ParseState
        {
            Initial,
            Unquoted,
            SingleQuoted,
            DoubleQuoted,
            Escaped,
        }
        List<string> args;
        const char DQUOT = '"';
        const char SQUOT = '\'';
        const char BACKSLASH = '\\';
        const char SPACE = ' ';

        public Arglist(string cmd)
        {
            args = new List<string>();
            Parse(cmd);
        }

        void Push(string arg)
        {
            args.Add(arg);
        }

        public string Shift()
        {
            string arg = null;
            if (args.Count > 0) {
                arg = args.ElementAt(0);
                args.RemoveAt(0);
            }
            return arg;
        }

        public string Join(string sep=" ")
        {
            StringBuilder cmd = new StringBuilder();
            bool first = true;
            string arg = Shift();
            while (arg != null) {
                if (!first) cmd.Append(sep);
                cmd.Append(Escape(arg));
                arg = Shift();
                first = false;
            }
            return cmd.ToString();
        }

        string Escape(string arg, string sep=" ")
        {
            arg.Replace("\"", "\\\"");
            arg.Replace("\n", "\\n");
            if (arg.Contains(sep)) {
                arg = $"\"{arg}\"";
            }
            return arg;
        }

        void Parse(string cmd)
        {
            Stack<ParseState> pstate = new Stack<ParseState>();
            StringBuilder arg = new StringBuilder();
            int cpos = 0;
            pstate.Push(ParseState.Initial);
            while (cpos < cmd.Length) {
                char c = cmd[cpos];
                ParseState state = pstate.Peek();
                switch (c) {
                    case DQUOT:
                        if (state == ParseState.Initial) {
                            pstate.Push(ParseState.DoubleQuoted);
                        } else if (state == ParseState.DoubleQuoted) {
                            pstate.Pop();
                        } else {
                            if (state == ParseState.Escaped) pstate.Pop();
                            arg.Append(c);
                        }
                        break;
                    case BACKSLASH:
                        if (state == ParseState.Escaped) {
                            arg.Append(BACKSLASH);
                            pstate.Pop();
                        } else {
                            pstate.Push(ParseState.Escaped);
                        }
                        break;
                    case SQUOT:
                        if (state == ParseState.Initial) {
                            pstate.Push(ParseState.SingleQuoted);
                        } else if (state == ParseState.SingleQuoted) {
                            pstate.Pop();
                        } else {
                            if (state == ParseState.Escaped) pstate.Pop();
                            arg.Append(c);
                        }
                        break;

                    case 'x':
                        if (state == ParseState.Escaped) {
                            if (ParseHex(cmd, 2, ref cpos, out string ch)) {
                                arg.Append(ch);
                            } else {
                                arg.Append(BACKSLASH);
                                arg.Append(c);
                                pstate.Pop();
                            }
                        } else {
                            arg.Append(c);
                        }
                        break;
                    case 'u':
                        if (state == ParseState.Escaped) {
                            if (ParseHex(cmd, 4, ref cpos, out string ch4)) {
                                arg.Append(ch4);
                            } else {
                                arg.Append(BACKSLASH);
                                arg.Append(c);
                                pstate.Pop();
                            }
                        } else {
                            arg.Append(c);
                        }
                        break;
                    case 'U':
                        if (state == ParseState.Escaped) {
                            if (ParseHex(cmd, 4, ref cpos, out string ch8)) {
                                arg.Append(ch8);
                            } else {
                                arg.Append(BACKSLASH);
                                arg.Append(c);
                                pstate.Pop();
                            }
                        } else {
                            arg.Append(c);
                        }
                        break;
                    case SPACE:
                        if (state == ParseState.Initial) {
                            Push(arg.ToString());
                            arg = new StringBuilder();
                        } else {
                            if (state == ParseState.Escaped) pstate.Pop();
                            arg.Append(c);
                        }
                        break;

                    default:
                        if (state == ParseState.Escaped) {
                            if (c == 'n') arg.Append('\n');
                            else if (c == 't') arg.Append('\t');
                            else if (c == 'b') arg.Append('\b');
                            else arg.Append(c);
                            pstate.Pop();
                        } else {
                            arg.Append(c);
                        }
                        
                        break;
                }
                cpos++;
            }
            if (arg.Length > 0) { Push(arg.ToString()); }
        }

        private bool ParseHex(string cmd, int len, ref int cpos, out string ch)
        {
            var sb = new StringBuilder();
            for (int i = 0; i < len; i++) {
                var c = cmd[cpos + 1 + i];
                if ((c >= '0' && c <= '9') || (c >= 'A' && c <= 'F') || (c >= 'a' && c <= 'f')) {
                    sb.Append(c);
                } else {
                    ch = "";
                    return false;
                }
            }
            cpos += len;
            ch = Char.ConvertFromUtf32(int.Parse(sb.ToString(), System.Globalization.NumberStyles.HexNumber));
            return true;
        }
    }
}
