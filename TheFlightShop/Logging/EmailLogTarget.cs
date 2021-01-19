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
        protected override void Write(IList<AsyncLogEventInfo> logEvents)
        {
            if (logEvents != null && logEvents.Any())
            {
                var logMessages = new List<string> { $"The error messages below reflect errors that occurred recently:<br />-------------------------------------" };
                logMessages.AddRange(logEvents.Select(logEvent => RenderLogEvent(Layout, logEvent.LogEvent)));

                var combinedLogMessage = string.Join($"<br /><br />", logMessages);
                var writeToServer = WriteToServer(combinedLogMessage);
                writeToServer.Wait();
            }
        }

        private async Task WriteToServer(string logMessage)
        {
            var emailApiKey = Environment.GetEnvironmentVariable("EMAIL_API_KEY");
            var username = Environment.GetEnvironmentVariable("EMAIL_FROM_USERNAME");
            var emailDomain = Environment.GetEnvironmentVariable("EMAIL_DOMAIN");
            var adminAddress = Environment.GetEnvironmentVariable("EMAIL_ADMIN_ADDRESS");
            var client = new MailgunEmailClient(emailApiKey, username, "(Automated) FlightShop Web Server", emailDomain);
            await client.SendEmail(adminAddress, "ERROR(s) occurred on website.", logMessage);
        }
    }
}
