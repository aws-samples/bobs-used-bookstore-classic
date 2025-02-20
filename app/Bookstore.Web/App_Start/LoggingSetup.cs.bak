using BobsBookstoreClassic.Data;
using Bookstore.Common;
using NLog;
using NLog.AWS.Logger;
using NLog.Config;
using NLog.Targets;

namespace Bookstore.Web
{
    public static class LoggingSetup
    {
        public static void ConfigureLogging()
        {
            var config = new LoggingConfiguration();

            Target loggingTarget;

            if (BookstoreConfiguration.Get("Services/LoggingService") == "aws")
            {
                loggingTarget = new AWSTarget { LogGroup = Constants.AppName };
            }
            else
            {
                loggingTarget = new DebuggerTarget();
            }

            config.AddTarget("aws", loggingTarget);

            config.LoggingRules.Add(new LoggingRule("*", LogLevel.Info, loggingTarget));

            LogManager.Configuration = config;
        }
    }
}