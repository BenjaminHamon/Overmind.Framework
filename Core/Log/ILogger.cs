using System;

namespace Overmind.Core.Log
{
	public interface ILogger
	{
		void Log(LogLevel level, string messageFormat, params string[] arguments);
		void Log(LogLevel level, Exception exception);
		void Log(LogLevel level, Exception exception, string messageFormat, params string[] arguments);

		void LogVerbose(string message);
		void LogInfo(string message);
		void LogWarning(string message);
		void LogError(string message);
	}
}
