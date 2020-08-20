using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TheFlightShop.DAL.Schemas;

namespace TheFlightShop.IO
{
    public class AwsS3FileManager : IFileManager
    {
        private const string CATEGORY_DIR = "category";
        private const string PRODUCT_DIR = "product";
        private const string DRAWING_DIR = "drawing";

        private readonly string _accessKeyId;
        private readonly string _secretAccessKey;
        private readonly string _bucketName;
        private readonly Amazon.RegionEndpoint _region;
        private readonly ILogger _logger;

        public AwsS3FileManager(string accessKeyId, string secretAccessKey, string bucketName, string regionName, ILogger logger)
        {
            _accessKeyId = accessKeyId;
            _secretAccessKey = secretAccessKey;
            _bucketName = bucketName;
            _region = Amazon.RegionEndpoint.GetBySystemName(regionName);
            _logger = logger;
        }

        public async Task<Stream> GetCategoryImage(string fileName)
        {
            return await GetFileStream($"{CATEGORY_DIR}/{fileName}");
        }

        public async Task<Stream> GetProductDrawing(string fileName)
        {
            return await GetFileStream($"{DRAWING_DIR}/{fileName}");
        }

        public async Task<Stream> GetProductImage(string fileName)
        {
            return await GetFileStream($"{PRODUCT_DIR}/{fileName}");
        }

        private async Task<Stream> GetFileStream(string key)
        {
            var stream = new MemoryStream();

            try
            {
                var stopwatch = Stopwatch.StartNew();
                using (var client = new AmazonS3Client(_accessKeyId, _secretAccessKey, _region))
                {
                    var response = await client.GetObjectAsync(_bucketName, key);

                    using (var responseStream = response.ResponseStream)
                    {
                        responseStream.CopyTo(stream);
                    }
                }

                stopwatch.Stop();
                _logger.LogInformation($"{nameof(AwsS3FileManager)}.{nameof(GetFileStream)}-fetched key={key} in {stopwatch.ElapsedMilliseconds}ms");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(AwsS3FileManager)}.{nameof(GetFileStream)}-failed to get stream for key={key}");
            }

            return stream;
        }

        public async Task<bool> OverwriteCategoryImage(IFormFile image)
        {
            var contentType = GetImageContentType(image.FileName);
            return await OverwriteFile(image, CATEGORY_DIR, contentType);
        }

        public async Task<bool> OverwriteProductDrawing(IFormFile drawing)
        {
            return await OverwriteFile(drawing, DRAWING_DIR, "application/pdf");
        }

        public async Task<bool> OverwriteProductImage(IFormFile image)
        {
            var contentType = GetImageContentType(image.FileName);
            return await OverwriteFile(image, PRODUCT_DIR, contentType);
        }

        private async Task<bool> OverwriteFile(IFormFile file, string containingDirectory, string contentType)
        {
            bool succeeded = false;

            try
            {                
                using (var client = new AmazonS3Client(_accessKeyId, _secretAccessKey, _region))
                {
                    using (var fileStream = file.OpenReadStream())
                    {
                        var transferRequest = new TransferUtilityUploadRequest
                        {
                            BucketName = _bucketName,
                            InputStream = fileStream,
                            Key = $"{containingDirectory}/{file.FileName}",
                            ContentType = contentType
                        };
                        var fileTransfer = new TransferUtility(client);
                        await fileTransfer.UploadAsync(transferRequest);

                        succeeded = true;
                    };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(AwsS3FileManager)}.{nameof(OverwriteFile)}-failed to overwrite key={containingDirectory}/{file?.FileName}");
                throw;
            }

            return succeeded;
        }

        private string GetImageContentType(string fileName)
        {
            var fileType = fileName.Split('.')[1];
            return $"image/{fileType}";
        }

        public async Task DeleteProductFiles(IEnumerable<Product> products)
        {
            var filenames = new List<string>(products.Select(product => product.ImageFilename));
            filenames.AddRange(products.Select(product => product.DrawingFilename));

            var keys = new List<string>();
            foreach (var product in products)
            {
                var imageKey = $"{PRODUCT_DIR}/{product.ImageFilename}";
                keys.Add(imageKey);
                var drawingKey = $"{DRAWING_DIR}/{product.DrawingFilename}";
                keys.Add(drawingKey);
            }
            await DeleteFiles(keys);
        }

        public async Task DeleteCategoryImage(string fileName)
        {
            var keys = new List<string> { $"{CATEGORY_DIR}/{fileName}" };
            await DeleteFiles(keys);
        }

        private async Task DeleteFiles(IEnumerable<string> keys)
        {
            try
            {
                using (var client = new AmazonS3Client(_accessKeyId, _secretAccessKey, _region))
                {
                    var deleteRequest = new DeleteObjectsRequest { BucketName = _bucketName };
                    foreach (var key in keys)
                    {
                        deleteRequest.AddKey(key);
                    }
                    await client.DeleteObjectsAsync(deleteRequest);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(AwsS3FileManager)}.{nameof(DeleteFiles)}-failed to delete keys={string.Join(",", keys ?? new List<string>())}");
                throw;
            }
        }
    }
}
