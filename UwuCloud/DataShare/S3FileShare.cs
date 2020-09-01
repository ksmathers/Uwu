using Amazon;
using Amazon.S3;
using Amazon.S3.Transfer;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

namespace UwuCloud.DataShare
{
    public class S3FileShare : IFileShare
    {
        const int KB = 1024;
        const int MB = 1024 * KB;
        const int GB = 1024 * MB;

        DataShareConfig cfg;
        AmazonS3Client s3client;

        string shareName;

        string access_id;
        string secret_key;

        public S3FileShare(DataShareConfig cfg, string shareName)
        {
            this.cfg = cfg;
            this.shareName = shareName;
            cfg.GetKeysForShare(shareName, out access_id, out secret_key);
            Debug.Assert(access_id != null);
            Debug.Assert(secret_key != null);
        }

        public Task<bool> Download(string key, string path)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> Upload(string key, string path)
        {
            var s3credentials = new Amazon.Runtime.BasicAWSCredentials(access_id, secret_key);
            //var s3config = new AmazonS3Config();
            //s3config.ProxyHost = "";
            //s3config.ProxyPort = 8080;
            var ok = false;
            using (s3client = new AmazonS3Client(s3credentials, RegionEndpoint.USWest2)) {
                ok = await UploadFileAsync(key, path);
            }
            return ok;
        }

        private async Task<bool> UploadFileAsync(string keyName, string filePath)
        {
            var bucketName = cfg.S3BucketForShare(shareName);
            try {
                var fileTransferUtility =
                    new TransferUtility(s3client);

                var fileTransferUtilityRequest = new TransferUtilityUploadRequest {
                    BucketName = bucketName,
                    FilePath = filePath,
                    StorageClass = S3StorageClass.OneZoneInfrequentAccess,
                    PartSize = 6 * MB,
                    Key = keyName,
                    CannedACL = S3CannedACL.Private
                };

                await fileTransferUtility.UploadAsync(fileTransferUtilityRequest);
                //MessageBox.Show("Upload completed");
                return true;
            } catch (AmazonS3Exception e) {
                throw (e);
                //MessageBox.Show("Error encountered on server. Message:'{0}' when writing an object", e.Message);
            } catch (Exception e) {
                throw (e);
                //MessageBox.Show("Unknown encountered on server. Message:'{0}' when writing an object", e.Message);
            }
        }
    }
}
