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
        public string Bucket { get; set; }
        [RequiredParameter]
        public string Directory { get; set; }
        [RequiredParameter]
        public string AccessKeyId { get; set; }
        [RequiredParameter]
        public string SecretAccessKey { get; set; }
        [RequiredParameter]
        public string Region { get; set; }

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
            var timestamp = DateTime.UtcNow.ToString("yyyyMMdd-HH:mm:ss:fff");
            var randomKey = new Random().Next(1000).ToString().PadLeft(3, '0');
            using (var client = new AmazonS3Client(AccessKeyId, SecretAccessKey, RegionEndpoint.GetBySystemName(Region)))
            {
                var messageBytes = Encoding.UTF8.GetBytes(logMessage);
                var messageStream = new MemoryStream(messageBytes);
                var request = new PutObjectRequest()
                {
                    BucketName = Bucket,
                    Key = $"{Directory}/{timestamp}_{randomKey}.txt",
                    ContentType = "text/plain",
                    InputStream = messageStream
                };
                await client.PutObjectAsync(request);
            }
        }
    }
}
