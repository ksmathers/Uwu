using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using System.Xml;
using Uwu.Config;

namespace Uwu.Core
{
    public abstract class ConfigIni
    {
        protected static ConfigIni _instance;
        protected IniFile ini;

        /// <summary>
        /// Basic constructor.  For applications create a subclass of ConfigIni with extension attributes for your 
        /// configuration options
        /// </summary>
        /// <param name="application">Application name</param>
        /// <param name="resource">Local machine name or other node dependent ID</param>
        /// <param name="defaultConfig">Default to use if the config file is missing</param>
        protected ConfigIni(string application, string resource, string defaultConfig)
        {
            ini = new IniFile(application, resource);
            if (!ini.Load()) {
                ini.ParseString(defaultConfig);
            }
            _instance = this;
        }

        /// <summary>
        /// Copy constructor used for adapting a loaded INI file with library specific configuration keys
        /// </summary>
        /// <param name="orig"></param>
        protected ConfigIni(ConfigIni orig)
        {
            ini = orig.ini;
        }

        public static T Instance<T>() where T : ConfigIni {
            return _instance as T;
        }

        public string ConnectTo {
            get { return ini.GetDefault("connectTo", "dbus:localhost"); }
            set { ini.SetDefault("connectTo", value); }
        }

        public IEnumerable<string> SectionKeys(string section)
        {
            foreach (var key in ini.Data.SectionKeys(section)) yield return key;
        }

        public string Interpolate(string raw, Dictionary<string,string> localVars=null)
        {
            return ini.Interpolate(raw, localVars);
        }
    }
}
