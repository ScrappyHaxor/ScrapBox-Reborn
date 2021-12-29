using System;
using System.IO;
using System.Collections.Generic;

namespace ScrapBox.Framework.Services
{
    public enum Severity
    {
        CRITICAL,
        ERROR,
        WARNING,
        INFO
    }

    public static class LogService
    {
        private static readonly List<string> logBuffer;

        static LogService()
        {
            logBuffer = new List<string>();
        }

        public static void Out(object obj)
        {
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.Write("DEV OUT -> ");
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write($"{obj}\n\n");
        }

        public static void Log(string assemblyName, string objectName, string source, string message, Severity severity)
        {
            ConsoleColor oldColor = Console.ForegroundColor;
            string date = $"{DateTime.Now:HH:mm:ss}";
            Console.Write($"{date} ");

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write($"[{assemblyName}] ");

            switch (severity)
            {
                case Severity.CRITICAL:
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    break;
                case Severity.ERROR:
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;
                case Severity.WARNING:
                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                    break;
                case Severity.INFO:
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    break;
            }

            Console.Write($"[{severity}] ");
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.Write($"[{objectName}] ");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write($"-{source}-: ");
            Console.ForegroundColor = oldColor;
            Console.Write($"{message}\n");

            string fullMessage = $"{date} [{assemblyName}] [{severity}] [{objectName}] -{source}-: {message}";
            logBuffer.Add(fullMessage);
        }

        internal static void GenerateLog()
        {
            StreamWriter writer = new StreamWriter("latest.txt");
            foreach (string s in logBuffer)
            {
                writer.WriteLine(s);
            }

            writer.Flush();
            writer.Close();
            writer.Dispose();
        }

        internal static void Log(string objectName, string source, string message, Severity severity)
        {
            Log("ScrapBox", objectName, source, message, severity);
        }

    }
}
