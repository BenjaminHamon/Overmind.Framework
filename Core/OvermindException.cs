using System;

namespace Overmind.Core
{
	public class OvermindException : Exception
	{
		public OvermindException()
			: base()
		{ }

		public OvermindException(string message)
			: base(message)
		{ }

		public OvermindException(string message, Exception innerException)
			: base(message, innerException)
		{ }
	}
}
