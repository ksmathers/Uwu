using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace UwuCloud.DataShare
{
    /// <summary>
    /// Upload and download zipped folders to S3.   
    /// </summary>
    public class DataShare
    {
        DataShareConfig cfg;
        public string Status;
        public string ShareName { set; get; }

        public event EventHandler StatusChanged;

        /// <summary>
        /// Upload or download data from S3 to a local filesystem folder.   Contents of the folder are zipped or unzipped 
        /// and transferred asynchronously to the share.
        /// 
        /// For a named share the INI file should contain a section named [Share:<paramref name="shareName"/>] with appropriate
        /// keys for the download directory and s3 bucket names.   Keys should be stored separately in an application specific keys
        /// INI file.
        ///    
        /// </summary>
        /// <param name="_cfg">loaded INI file</param>
        /// <param name="shareName">name of the share</param>
        public DataShare(Uwu.Core.ConfigIni _cfg, string shareName)
        {
            cfg = new DataShareConfig(_cfg);
            ShareName = shareName;
        }

        protected void OnStatusChanged(string newstatus)
        {
            Status = newstatus;
            StatusChanged?.Invoke(this, null);
        }

        public IEnumerable<string> FileList(string shareName, string folderName)
        {
            var zip = new FolderZip(cfg, shareName, folderName);
            return zip.FolderFiles(folderName);
        }

        public async Task<bool> Upload(string folderName)
        {
            string shareName = ShareName;
            var zip = new FolderZip(cfg, shareName, folderName);
            OnStatusChanged("PACKAGING...");
            await zip.pack();
            var zipPath = zip.zipFile;
            var zipFile = System.IO.Path.GetFileName(zipPath);
            var share = new S3FileShare(cfg, shareName);
            OnStatusChanged("UPLOADING...");
            var success = await share.Upload(zipFile, zipPath);
            if (success) {
                OnStatusChanged("SUCCESS");
            } else {
                OnStatusChanged("FAILED");
            }
            return success;
        }
    }
}
