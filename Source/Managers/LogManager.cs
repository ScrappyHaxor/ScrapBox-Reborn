using System;

namespace ScrapBox.Managers
{
    public class LogMessage
    {
        public enum Severity
        {
            CRITICAL,
            ERROR,
            WARNING,
            INFO,
            VERBOSE
        }

        public string Source { get; set; }
        public string Message { get; set; }
        public Severity LogSeverity { get; set; }

        public LogMessage(string source, string message, Severity severity)
        {
            Source = source;
            Message = message;
            LogSeverity = severity;
        }
    }

    public static class LogManager
    {
        public static void Log(LogMessage message)
        {
            ConsoleColor oldColor = Console.ForegroundColor;
            switch (message.LogSeverity)
            {
                case LogMessage.Severity.CRITICAL:
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    break;
                case LogMessage.Severity.ERROR:
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;
                case LogMessage.Severity.WARNING:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    break;
                case LogMessage.Severity.INFO:
                    Console.ForegroundColor = ConsoleColor.White;
                    break;
                case LogMessage.Severity.VERBOSE:
                    Console.ForegroundColor = ConsoleColor.Gray;
                    break;
            }

            Console.WriteLine($"[{DateTime.Now.ToString("HH:mm:ss")}] [{message.LogSeverity.ToString()}] [{message.Source}]: {message.Message}");
            Console.ForegroundColor = oldColor;
        }

    }
}
