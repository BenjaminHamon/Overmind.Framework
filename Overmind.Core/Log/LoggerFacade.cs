namespace Overmind.Core.Log
{
    public static class LoggerFacade
    {
        public static ILogger Logger;

        public static void LogVerbose(string message) { Logger.LogVerbose(message); }
        public static void LogInfo(string message) { Logger.LogInfo(message); }
        public static void LogWarning(string message) { Logger.LogWarning(message); }
        public static void LogError(string message) { Logger.LogError(message); }
    }
}
