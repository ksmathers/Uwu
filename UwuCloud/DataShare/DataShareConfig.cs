using System;
using System.Collections.Generic;
using System.Text;

namespace UwuCloud.DataShare
{
    public class DataShareConfig : Uwu.Core.ConfigIni
    {
        public DataShareConfig(Uwu.Core.ConfigIni baseConfig)
            : base(baseConfig)
        {
            // Overlays DataShare accessors on an existing loaded configuration file
        }

        // Subconfiguration for access keys
        class PrivateKeys
        {
            Uwu.Config.IniFile inidata;
            string inifile;
            const string GLOBAL = "defaults";
            public PrivateKeys(string application, string role)
            {
                inifile = System.IO.Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    String.Format("{0}.keys", role));
                inidata = new Uwu.Config.IniFile(application, role, inifile);
            }

            public string SecretKey {
                get { return inidata.GetDefault("SECRET_KEY", ""); }
                set { inidata.SetDefault("SECRET_KEY", value); }
            }

            public string AccessId {
                get { return inidata.GetDefault("ACCESS_ID", ""); }
                set { inidata.SetDefault("ACCESS_ID", value); }
            }

            public void SaveConfig()
            {
                inidata.Save();
            }
        }



        // The location where CSV files are stored
        public string DownloadDirectory {
            get { return ini.GetDefault("downloads", @"%DOWNLOADS%"); }
        }

        public List<string> ShareNames {
            get {
                var result = new List<string>();
                foreach (var section in ini.Sections) {
                    var exp = ShareNameFromSection(section);
                    if (exp != null) {
                        result.Add(exp);
                    }
                }
                return result;
            }
        }

        public List<string> ZipDataExtensions {
            get {
                string zipde = ini.GetDefault("zipdataextensions", ".xlsx,.csv,.wav");
                return new List<string>(zipde.Split(','));
            }
        }

        public string ShareSection(string shareName)
        {
            return String.Format("Share:{0}", shareName);
        }

        public string ShareNameFromSection(string section)
        {
            string prefix = "Share:";
            if (section.StartsWith(prefix)) {
                return section.Substring(prefix.Length);
            }
            return null;
        }

        // Directory where zip files are cached
        public string CacheDirectoryForShare(string shareName)
        {
            return ini.GetParam(ShareSection(shareName), "cacheDirectory", String.Format(@"%APPDATA%\.{0}", shareName));
        }

        public List<string> ChecklistForShare(string shareName)
        {
            return new List<string>(ini.GetParam(ShareSection(shareName), "checkList", "").Split(','));
        }

        public string S3BucketForShare(string shareName)
        {
            return ini.GetParam(ShareSection(shareName), "s3bucket", "");
        }

        public void GetKeysForShare(string shareName, out string access_id, out string secret_key)
        {
            PrivateKeys pkeys = new PrivateKeys(ini.Application, shareName);
            access_id = pkeys.AccessId;
            secret_key = pkeys.SecretKey;
        }

        public void AddShareS3(string shareName, string s3bucket, string cacheDir, string access_id, string secret_key)
        {
            var section = ShareSection(shareName);
            ini.SetParam(section, "shareName", shareName);
            ini.SetParam(section, "s3bucket", s3bucket);
            ini.SetParam(section, "cacheDirectory", cacheDir);
            PrivateKeys pkeys = new PrivateKeys(ini.Application, shareName);
            pkeys.AccessId = access_id;
            pkeys.SecretKey = secret_key;
            pkeys.SaveConfig();
        }


    }
}
