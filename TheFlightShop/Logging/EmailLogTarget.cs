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
using TheFlightShop.Email;

namespace TheFlightShop.Logging
{
    [Target("EmailLogTarget")]
    public sealed class EmailLogTarget : TargetWithLayout
    {
        [RequiredParameter]
        public string TargetEmailAddress { get; set; }
        [RequiredParameter]
        public string EmailClientDomain { get; set; }
        [RequiredParameter]
        public string EmailClientUsername { get; set; }
        [RequiredParameter]
        public string EmailClientApiKey { get; set; }

        protected override void Write(IList<AsyncLogEventInfo> logEvents)
        {
            if (logEvents != null && logEvents.Any())
            {
                var logMessages = logEvents.Select(logEvent => RenderLogEvent(Layout, logEvent.LogEvent));
                var combinedLogMessage = string.Join($"The error messages below reflect errors that occurred recently:{Environment.NewLine}-------------------------------------{Environment.NewLine}{Environment.NewLine}", logMessages);
                var writeToServer = WriteToServer(combinedLogMessage);
                writeToServer.Wait();
            }
        }

        private async Task WriteToServer(string logMessage)
        {
            var client = new MailgunEmailClient(EmailClientApiKey, EmailClientUsername, "(Automated) FlightShop Web Server", EmailClientDomain);
            await client.SendEmail(TargetEmailAddress, "ERROR(s) occurred on website.", logMessage);
        }
    }
}
