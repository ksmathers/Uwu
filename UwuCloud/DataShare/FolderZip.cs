using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Threading.Tasks;

namespace UwuCloud.DataShare
{
    public class FolderZip
    {
        ZipArchive zip;
        DataShareConfig cfg;

        string folderName;
        string shareName;

        public FolderZip(DataShareConfig cfg, string shareName, string folderName)
        {
            this.cfg = cfg;
            this.folderName = folderName;
            this.shareName = shareName;
        }

        public string zipFile {
            get {
                var path = Path.Combine(cfg.S3BucketForShare(shareName),
                    String.Format("{0}-{1}.zip", shareName, folderName));
                return cfg.Interpolate(path);
            }
        }

        string recordingDirFile(string fname)
        {
            return Path.Combine(cfg.Interpolate(cfg.DownloadDirectory), fname);
        }

        public IEnumerable<string> FolderFiles(string folderName)
        {
            var prefix = string.Format("{0}-{1}-", shareName, folderName);

            var suffix = cfg.ZipDataExtensions;
            foreach (var path in Directory.EnumerateFiles(Path.Combine(cfg.Interpolate(cfg.DownloadDirectory), folderName))) {
                var fname = Path.GetFileName(path);
                if (fname.StartsWith(prefix)) {
                    var include = false;
                    foreach (var s in suffix) {
                        if (fname.EndsWith(s)) include = true;
                    }
                    if (include) {
                        yield return fname;
                    }
                }
            }
        }

        public void cacheInit(string cacheDir)
        {
            if (Directory.Exists(cacheDir)) return;

            string preDir = Path.GetDirectoryName(cacheDir);
            if (!Directory.Exists(preDir)) {
                cacheInit(preDir);
            }
            Directory.CreateDirectory(cacheDir);
        }

        public async Task pack()
        {
            cacheInit(Path.GetDirectoryName(zipFile));
            var worker = Task.Run(() =>
            {
                var ostream = new FileStream(zipFile, FileMode.Create);
                zip = new ZipArchive(ostream, ZipArchiveMode.Create);
                foreach (var fname in FolderFiles(folderName)) {
                    var entry = zip.CreateEntry(fname);
                    using (var fout = entry.Open()) {
                        var fin = System.IO.File.OpenRead(recordingDirFile(fname));
                        fin.CopyTo(fout);
                    }
                }
                zip.Dispose();
                ostream.Close();
            }
                );
            await worker;
        }

        public void unpack()
        {
            throw new NotImplementedException();
        }
    }
}
