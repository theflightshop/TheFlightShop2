using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using NLog;
using NLog.Common;
using NLog.Config;
using NLog.Targets;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TheFlightShop.Logging
{
    [Target("AmazonS3LogTarget")]
    public sealed class AmazonS3LogTarget : TargetWithLayout
    {
        [RequiredParameter]
        public string Directory { get; set; }

        protected override void Write(IList<AsyncLogEventInfo> logEvents)
        {
            if (logEvents != null && logEvents.Any())
            {
                var logMessages = logEvents.Select(logEvent => RenderLogEvent(Layout, logEvent.LogEvent));
                var combinedLogMessage = string.Join($"{Environment.NewLine}{Environment.NewLine}", logMessages);
                var writeToServer = WriteToServer(combinedLogMessage);
                writeToServer.Wait();
            }
        }

        private async Task WriteToServer(string logMessage)
        {
            var accessKeyId = Environment.GetEnvironmentVariable("S3_ACCESS_KEY_ID");
            var secretAccessKey = Environment.GetEnvironmentVariable("S3_SECRET_ACCESS_KEY");
            var s3BucketName = Environment.GetEnvironmentVariable("S3_PRODUCT_CONTENT_BUCKET_NAME");
            var awsRegion = Environment.GetEnvironmentVariable("S3_PRODUCT_CONTENT_REGION");

            using (var client = new AmazonS3Client(accessKeyId, secretAccessKey, RegionEndpoint.GetBySystemName(awsRegion)))
            {
                var messageBytes = Encoding.UTF8.GetBytes(logMessage);
                var messageStream = new MemoryStream(messageBytes);
                var request = new PutObjectRequest()
                {
                    BucketName = s3BucketName,
                    Key = $"{Directory}/{GetFilename(logMessage)}.txt",
                    ContentType = "text/plain",
                    InputStream = messageStream
                };
                await client.PutObjectAsync(request);
            }
        }

        private string GetFilename(string logMessage)
        {
            var timestamp = DateTime.UtcNow.ToString("yyyyMMdd-HH:mm:ss:fff");
            var randomKey = new Random().Next(1000).ToString().PadLeft(3, '0');
            var filename = $"{timestamp}_{randomKey}";
            var errorIdSplit = logMessage.Split(LoggingConstants.ERROR_ID_PREFIX);
            if (errorIdSplit.Length > 1)
            {
                errorIdSplit = errorIdSplit[1].Split(LoggingConstants.ERROR_ID_SUFFIX);
                if (errorIdSplit.Length > 1)
                {
                    filename = $"{errorIdSplit[0]}_{filename}";
                }
            }
            return filename;
        }
    }
}
