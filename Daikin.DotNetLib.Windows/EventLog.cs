using System.Diagnostics;
using System.Reflection;

namespace Daikin.DotNetLib.Windows
{
    public static class EventLog
    {
        #region Enumerators
        public enum LogLocation
        {
            Application = 0,
            Security = 1,
            Setup = 2,
            System = 3,
            ForwardedEvents = 4
        }
        #endregion

        #region Functions
        public static void Post(string source, int statusCode, string message, EventLogEntryType eventType = EventLogEntryType.Information, LogLocation logLocation = LogLocation.Application)
        {
            var logLocationTypes = new[] { "Application", "Security", "Setup", "System", "Forwarded Events" };
            var log = logLocationTypes[(int)logLocation];

            if (source == null)
            {
                try
                {
                    var assembly = Assembly.GetEntryAssembly() ?? Assembly.GetCallingAssembly();
                    source = assembly.GetName().FullName;
                }
                catch
                {
                    source = string.Empty;
                }
            }

            if (!System.Diagnostics.EventLog.SourceExists(source))
            {
                System.Diagnostics.EventLog.CreateEventSource(source, log); // Now this source is assigned to that log going forward
                var logSource = new System.Diagnostics.EventLog {Source = source};
                logSource.WriteEntry($"New Source '{source}' created", EventLogEntryType.Information);
            }

            System.Diagnostics.EventLog.WriteEntry(source, message, eventType, statusCode);
        }

        public static bool DeleteSource(string source)
        {
            try
            {
                System.Diagnostics.EventLog.DeleteEventSource(source);
                return true;
            }
            catch
            {
                return false;
            }
        }
        #endregion
    }
}
