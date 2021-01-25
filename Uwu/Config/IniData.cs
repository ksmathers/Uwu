using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using Uwu.Core;

namespace Uwu.Config
{
    public class IniData
    {
        List<IniRow> rows;
        Regex reSection;
        Regex reComment;

        /// <summary>
        /// A container for an ordered list lines read from an INI file.   
        /// </summary>
        /// <param name="istream"></param>
        public IniData()
        {
            rows = new List<IniRow>();        
            reSection = new Regex(@"\s*\[([^]]+)\]\s*");
            reComment = new Regex(@"\s*[;](.*)"); 
        }

        public void LoadStream(System.IO.Stream istream)
        {
            TextReader reader = new StreamReader(istream);
            while (ParseLine(reader));
        }

        public void SaveStream(System.IO.Stream ostream)
        {
            TextWriter writer = new StreamWriter(ostream);
            for (int i = 0; i < rows.Count; i++) {
                var row = rows[i];
                writer.WriteLine(row.Serialize());
            }
            writer.Close();
        }

        public void LoadString(string iniData)
        {
            var buf = ASCIIEncoding.UTF8.GetBytes(iniData);
            var istream = new MemoryStream(buf);
            LoadStream(istream);
        }

        public string SaveString()
        {
            var ostream = new MemoryStream();
            SaveStream(ostream);
            return ASCIIEncoding.UTF8.GetString(ostream.ToArray());
        }

        public int GetSectionIdx(string sectionName)
        {
            for (int i = 0; i < rows.Count; i++) {
                if (rows[i].AsSection() == sectionName) return i;
            }
            return -1;
        }

        public IEnumerable<string> Sections {
            get {
                for (int i = 0; i < rows.Count; i++) {
                    var section = rows[i].AsSection();
                    if (section != null) yield return section;
                }
            }
        }

        public int GetKeyValueIdx(string sectionName, string keyName)
        {
            int cpos = GetSectionIdx(sectionName);
            if (cpos < 0) return -1;
            cpos++;
            while (cpos < rows.Count) {
                var row = rows[cpos];
                if (row.AsSection() != null) return -1;
                var kv = row.AsKeyValue();
                if (kv != null) {
                    var key = kv?.Key;
                    if (key == keyName) {
                        return cpos;
                    }
                }
                cpos++;
            }
            return -1;
        }

        public IEnumerable<string> SectionKeys(string sectionName)
        {
            int cpos = GetSectionIdx(sectionName);
            if (cpos >= 0) {
                cpos++;
                while (cpos < rows.Count) {
                    var row = rows[cpos];
                    if (row.AsSection() != null) break;
                    var kv = row.AsKeyValue();
                    if (kv != null) {
                        yield return kv?.Key;
                    }
                    cpos++;
                }
            }
        }

        public string GetParam(string section, string key, string defaultValue = null)
        {
            int idx = GetKeyValueIdx(section, key);
            if (idx < 0) return defaultValue;
            return rows[idx].AsKeyValue().Value.Value;
        }

        public void SetParam(string section, string key, string value)
        {
            int idx = GetKeyValueIdx(section, key);
            if (idx < 0) {
                int cpos = AddSection(section);
                int insertAfter = cpos;
                while (rows[cpos].AsSection() == null) {
                    if (rows[cpos].AsKeyValue() != null) insertAfter = cpos;
                    cpos++;
                }
                rows.Insert(insertAfter + 1, IniRow.KeyValue(key, value, StringUtil.EscapeHint.Escaped));
            }
            if (idx >= 0) {
                rows[idx] = IniRow.KeyValue(key, value, StringUtil.EscapeHint.Escaped);
            } 
        }

        public int AddSection(string section)
        {
            int idx = GetSectionIdx(section);
            if (idx < 0) {
                rows.Add(IniRow.Section(section));
                idx = GetSectionIdx(section);
            }
            return idx;
        }



        public bool ParseLine(TextReader reader)
        {
            var line = reader.ReadLine();
            if (line == null) return false;
            Match m;
            m = reSection.Match(line);
            if (m.Success) {
                var sectionName = m.Groups[1].Value;
                rows.Add(IniRow.Section(sectionName));
                return true;
            }

            m = reComment.Match(line);
            if (m.Success) {
                var commentText = m.Groups[1].Value;
                rows.Add(IniRow.Comment(commentText));
                return true;
            }

            StringUtil.EscapeHint hint = StringUtil.EscapeHint.Escaped;
            if (line.Contains("=")) {
                var args = line.Split('=');
                string key = args[0].Trim();
                string value = args[1].Trim();
                if (value.StartsWith("\"\"\"")) {
                    if (!(value.Length > 3 && value.EndsWith("\"\"\""))) {
                        hint = StringUtil.EscapeHint.TripleDoubleQuoted;
                        value += ReadToTerminator("\"\"\"", reader);
                    }
                } else if (value.StartsWith("'''")) {
                    if (!(value.Length > 3 && value.EndsWith("'''"))) {
                        hint = StringUtil.EscapeHint.TripleQuoted;
                        value += ReadToTerminator("'''", reader);
                    }
                } else if (value.StartsWith("\"")) {
                    hint = StringUtil.EscapeHint.DoubleQuoted;
                } else if (value.StartsWith("'")) {
                    hint = StringUtil.EscapeHint.Quoted;
                } else if (value.EndsWith("\\")) {
                    value = value.TrimEnd("\\") + "\n" + ReadForContinuation("\\", reader);
                }
                rows.Add(IniRow.KeyValue(key, value.Dequote(), hint));
                return true;
            }
            rows.Add(IniRow.Invalid(line));
            return true;
        }

        string ReadToTerminator(string terminator, TextReader reader)
        {
            StringBuilder buf = new StringBuilder();
            for (; ; ) {
                string line = reader.ReadLine();
                if (line == null) break;
                buf.Append(line);
                if (line.Trim().EndsWith(terminator)) break;
                buf.Append("\n");
            }
            return buf.ToString();
        }

        string ReadForContinuation(string continuation, TextReader reader)
        {
            StringBuilder buf = new StringBuilder();
            bool continued = true;
            while (continued) {
                continued = false;
                string line = reader.ReadLine();
                if (line == null) break;
                line = line.Trim();
                if (line.EndsWith("\\")) {
                    continued = true;
                    line = line.TrimEnd("\\") + "\n";
                }
                buf.Append(line);
            }
            return buf.ToString();
        }
    }
}
