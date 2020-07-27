using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Uwu.Core;

namespace Uwu.Config
{
    internal class IniRow
    {
        internal enum RowType { Invalid, Section, KeyValue, Comment };

        RowType rowType;
        string arg0;
        string arg1;
        StringUtil.EscapeHint hint;

        internal KeyValuePair<string, string>? AsKeyValue()
        {
            if (rowType == RowType.KeyValue) {
                return new KeyValuePair<string, string>(arg0, arg1);
            }
            return null;
        }

        internal string AsSection()
        {
            if (rowType == RowType.Section) {
                return arg0;
            }
            return null;
        }

        internal IniRow(RowType type, string arg0, string arg1=null, StringUtil.EscapeHint hint = StringUtil.EscapeHint.None)
        {
            this.rowType = type;
            this.arg0 = arg0;
            this.arg1 = arg1;
            this.hint = hint;
        }

        internal static IniRow Section(string sectionName)
        {
            return new IniRow(RowType.Section, sectionName);
        }

        internal static IniRow KeyValue(string key, string value, StringUtil.EscapeHint escapeHint)
        {
            return new IniRow(RowType.KeyValue, key, value, escapeHint);
        }

        internal string Serialize()
        {
            string buf = "";
            switch (rowType) {
                case RowType.Comment:
                    buf = $";{arg0}";
                    break;
                case RowType.Invalid:
                    buf = arg0;
                    break;
                case RowType.KeyValue:
                    buf = $"{arg0}={arg1.ShellEscape(hint)}";
                    break;
                case RowType.Section:
                    buf = $"[{arg0}]";
                    break;
            }
            return buf;
        }

        internal static IniRow Comment(string comment)
        {
            return new IniRow(RowType.Comment, comment);
        }

        internal static IniRow Invalid(string linedata)
        {
            return new IniRow(RowType.Invalid, linedata);
        }
    }
}
