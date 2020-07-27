using System;
using System.Collections.Generic;
using System.Text;

namespace Uwu.Core
{
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
