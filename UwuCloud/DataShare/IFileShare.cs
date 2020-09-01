using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace UwuCloud.DataShare
{
    interface IFileShare
    {
        Task<bool> Upload(string key, string path);
        Task<bool> Download(string key, string path);
    }
}
