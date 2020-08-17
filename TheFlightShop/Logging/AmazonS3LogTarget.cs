using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using NLog;
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
    public sealed class AmazonS3LogTarget : AsyncTaskTarget
    {
        [RequiredParameter]
        public string Bucket { get; set; }
        [RequiredParameter]
        public string Directory { get; set; }
        [RequiredParameter]
        public string AccessKeyId { get; set; }
        [RequiredParameter]
        public string SecretAccessKey { get; set; }
        [RequiredParameter]
        public string Region { get; set; }

        protected override async Task WriteAsyncTask(LogEventInfo logEvent, CancellationToken cancellationToken)
        {
            var logMessage = RenderLogEvent(Layout, logEvent);
            await WriteToServer(logMessage);
        }

        private async Task WriteToServer(string logMessage)
        {
            var timestamp = DateTime.UtcNow.ToString("yyyyMMdd-HH:mm:ss:fff");
            var randomKey = new Random().Next(1000).ToString().PadLeft(3, '0');
            using (var client = new AmazonS3Client(AccessKeyId, SecretAccessKey, RegionEndpoint.GetBySystemName(Region)))
            {
                var messageBytes = Encoding.UTF8.GetBytes(logMessage);
                var messageStream = new MemoryStream(messageBytes);
                var request = new PutObjectRequest()
                {
                    BucketName = Bucket,
                    Key = $"{Directory}/{Name}_{timestamp}_{randomKey}.txt",
                    ContentType = "text/plain",
                    InputStream = messageStream
                };
                await client.PutObjectAsync(request);
            }
        }
    }
}
