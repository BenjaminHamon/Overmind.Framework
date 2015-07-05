namespace Overmind.Core.Log
{
    public interface ILogger
    {
        void LogVerbose(string message);
        void LogInfo(string message);
        void LogWarning(string message);
        void LogError(string message);
    }
}
