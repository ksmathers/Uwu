using System;
using System.Collections.Generic;
using System.Resources;
using System.Text;
using System.IO;
using Uwu.Core;
using System.Xml;
using System.Linq;

namespace Uwu.Config
{
    /// <summary>
    /// A container for the configuration information for an application, by default located in the %LOCALAPPDATA% directory
    /// with in a file named to match the application name.
    /// </summary>
    public class IniFile
    {
        string application;
        string default_inipath = $"%LOCALAPPDATA%\\%APPLICATION%.ini";
        string resource;
        string path;
        Dictionary<string, string> vars;
        IniData idata;

        public IniFile(string application, string resource = null, string inipath = null)
        {
            this.application = application;
            vars = new Dictionary<string, string>();
            vars.Add("APPLICATION", application);
            vars.Add("RESOURCE", resource);
            vars.Add("USER", System.Environment.UserName);
            vars.Add("HOSTNAME", System.Environment.MachineName);

            if (inipath == null) inipath = default_inipath;
            if (resource == null) resource = System.Environment.MachineName;
            this.path = inipath.Interpolate(vars);
            this.resource = resource;
            Load();
        }

        public IniData Data { get { return idata; } }
        public string Application { get { return application; } }

        public string GetParam(string section, string key, string defaultVal = null)
        {
            return Data.GetParam(section, key, defaultVal);
        }


        public string GetDefault(string key, string defaultValue = null)
        {

            var value = GetParam(resource, key);
            if (value == null) value = GetParam("default", key, defaultValue);
            return value;
        }

        public IEnumerable<string> Sections {
            get { return Data.Sections; }
        }

        public void SetParam(string section, string key, string value)
        {
            Data.SetParam(section, key, value);
        }

        public void SetDefault(string key, string value)
        {
            SetParam(resource, key, value);
        }

        internal void ReadStream(Stream istream)
        {
            idata = new IniData();
            idata.LoadStream(istream);
        }

        internal void WriteStream(Stream ostream)
        {
            idata.SaveStream(ostream);
        }

        /// <summary>
        /// Loads INI file from disk
        /// </summary>
        /// <returns>true if the file was present and was loaded, false otherwise</returns>
        public bool Load()
        {
            if (File.Exists(path)) {
                FileStream fi = new FileStream(path, FileMode.Open);
                ReadStream(fi);
                fi.Close();
                return true;
            }
            return false;
        }

        /// <summary>
        /// Save INI file to disk
        /// </summary>
        public void Save()
        {
            FileStream fo = new FileStream(path, FileMode.OpenOrCreate);
            fo.SetLength(0);
            idata.SaveStream(fo);
            fo.Close();
        }

        /// <summary>
        /// Convert INI file to string
        /// </summary>
        /// <returns></returns>
        override public string ToString()
        {
            return idata.SaveString();
        }

        /// <summary>
        /// Load INI file from an initialization string
        /// </summary>
        /// <param name="iniData"></param>
        public void ParseString(string iniData)
        {
            idata = new IniData();
            idata.LoadString(iniData);
        }

        public string Interpolate(string instr, Dictionary<string, string> localVars = null)
        {
            var tmp = instr.Interpolate(vars);
            if (localVars != null) {
                tmp = tmp.Interpolate(localVars);
            }
            return tmp;
        }
    }
}
