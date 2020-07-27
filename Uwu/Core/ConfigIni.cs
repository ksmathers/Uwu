using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Xml;
using Uwu.Config;

namespace Uwu.Core
{
    public abstract class ConfigIni
    {
        protected static ConfigIni _instance;
        protected IniFile ini;

        protected ConfigIni(string application, string resource, string defaultConfig)
        {
            ini = new IniFile(application, resource);
            if (!ini.Load()) {
                ini.ParseString(defaultConfig);
            }
            _instance = this;
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
    }
}
