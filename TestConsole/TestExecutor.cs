using Overmind.Framework.Core;
using System;
using System.Collections.Generic;

namespace Overmind.Framework.TestConsole
{
	public class TestExecutor
	{
		public TestExecutor(Shell shell)
		{
			this.shell = shell;
		}

		private readonly Shell shell;

		public void Void()
		{
			shell.Output.WriteLine("Executing Void");
		}

		public void Integer(int argument)
		{
			shell.Output.WriteLine("Executing Integer with argument {0}", argument);
		}

		public void Optional(string argument = null)
		{
			shell.Output.WriteLine("Executing Optional with argument {0}", argument == null ? "null" : "\"" + argument + "\"");
		}

		public void List(IList<string> argument)
		{
			shell.Output.WriteLine("Executing List with argument [{0}]", String.Join(", ", argument));
		}
	}
}
