using UnityEngine;

namespace Forge.Utilities
{
    public static class Logger
    {
        /// <summary>
        /// Specifies the type of log message.
        /// </summary>
        public enum LogType { Info, Warning, Error }

        /// <summary>
        /// Logs a message with the specified log type and source.
        /// </summary>
        /// <param name="type">The type of log message (Info, Warning, Error).</param>
        /// <param name="source">The source of the log message, typically indicating the module or class.</param>
        /// <param name="message">The log message content.</param>
        public static void Log(LogType type, string source, string message)
        {
            string formattedMessage = FormatMessage(type, source, message);

            switch (type)
            {
                case LogType.Info:
                    Debug.Log(formattedMessage);
                    break;
                case LogType.Warning:
                    Debug.LogWarning(formattedMessage);
                    break;
                case LogType.Error:
                    Debug.LogError(formattedMessage);
                    break;
            }
        }

        /// <summary>
        /// Formats a log message with its type and source.
        /// </summary>
        /// <param name="type">The type of log message.</param>
        /// <param name="source">The source of the log message.</param>
        /// <param name="message">The content of the log message.</param>
        /// <returns>A formatted string containing the log type, source, and message.</returns>
        private static string FormatMessage(LogType type, string source, string message)
        {
            return $"[{type}] [{source}] {message}";
        }
    }
}