using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using Bookstore.Domain;
using System.IO;
using System.Threading.Tasks;
using BobsBookstoreClassic.Data;

namespace Bookstore.Data.FileServices
{
    public class S3FileService : IFileService
    {
        private readonly TransferUtility transferUtility;

        public S3FileService(IAmazonS3 s3Client)
        {
            transferUtility = new TransferUtility(s3Client);
        }

        public async Task DeleteAsync(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath)) return;

            var bucketName = BookstoreConfiguration.Get("Files/BucketName");
            var request = new DeleteObjectRequest
            {
                BucketName = bucketName,
                Key = Path.GetFileName(filePath)
            };

            await transferUtility.S3Client.DeleteObjectAsync(request);
        }

        public async Task<string> SaveAsync(Stream contents, string filename)
        {
            if (contents == null) return null;

            var bucketName = BookstoreConfiguration.Get("Files/BucketName");
            var uniqueFilename = $"{Path.GetFileNameWithoutExtension(Path.GetRandomFileName())}{Path.GetExtension(filename)}";
            var cloudFrontDomain = BookstoreConfiguration.Get("Files/CloudFrontDomain");

            var request = new TransferUtilityUploadRequest
            {
                BucketName = bucketName,
                InputStream = contents,
                Key = uniqueFilename
            };

            await transferUtility.UploadAsync(request);

            return $"{cloudFrontDomain}/{uniqueFilename}";
        }
    }
}
