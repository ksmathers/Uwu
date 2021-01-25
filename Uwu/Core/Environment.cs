using System;
using System.Collections.Generic;
using System.Text;

namespace Uwu.Core
{
    /// <summary>
    /// A container class for accessing environment variables.   These include actual System environment
    /// variables together with a set of meta-environment variables some of which are automatically available
    /// like 'HOME', and 'APPDATA', and others of which can be set using SetVar().   Values set using SetVar() 
    /// override built in values and system environment variables.
    /// </summary>
    public class Environment
    {
        static Environment _instance;
        Dictionary<string, string> vars;

        static public Environment Instance {
            get {
                if (_instance == null) {
                    _instance = new Environment();
                }
                return _instance;
            }
        }



        public Environment()
        {
            vars = new Dictionary<string, string>();
        }

        /// <summary>
        /// Sets a program internal meta-environment variable.  If the value is null then the
        /// variable will be deleted if it exists.
        /// </summary>
        /// <param name="name">name</param>
        /// <param name="value">value or null</param>
        public void SetVar(string name, string value)
        {
            if (value == null) {
                if (vars.ContainsKey(name)) vars.Remove(name);
            } else {
                if (vars.ContainsKey(name)) {
                    vars[name] = value;
                } else {
                    vars.Add(name, value);
                }
            }
        }

        public string GetVar(string key)
        {
            string value = null;
            if (vars.ContainsKey(key)) return vars[key];

            switch (key) {
                case "HOME":
                    return System.Environment.GetFolderPath(System.Environment.SpecialFolder.UserProfile);
                case "APPDATA":
                    return System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData);
                case "LOCALAPPDATA":
                    return System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData);
                case "DESKTOP":
                    return System.Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop);
                case "SYSTEM":
                    return System.Environment.GetFolderPath(System.Environment.SpecialFolder.System);
                case "PROGRAM_FILES":
                    return System.Environment.GetFolderPath(System.Environment.SpecialFolder.ProgramFiles);
                case "DOCUMENTS":
                    return System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments);
                case "PICTURES":
                    return System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyPictures);
                case "DOWNLOADS":
                    return System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.UserProfile), "Downloads");
            }

            value = System.Environment.GetEnvironmentVariable(key);
            if (value != null) return value;

            return null;
        }
    }
}
