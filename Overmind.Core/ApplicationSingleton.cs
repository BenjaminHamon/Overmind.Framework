using Overmind.Core.Log;

namespace Overmind.Core
{
	/// <summary>
	/// <para>Application level object to access common dependencies such as logging.
	/// These dependencies are required by pretty much every class
	/// so this singleton provides a way to get the default instance for them.</para>
	/// <para>You must initialize this object at the start of your program.</para>
	/// </summary>
	public static class ApplicationSingleton
	{
		private static ILogger logger;
		public static ILogger Logger
		{
			get
			{
				if (logger == null)
					throw new OvermindException("ApplicationSingleton.Logger is not initialized.");
				return logger;
			}
			set { logger = value; }
		}
	}
}
