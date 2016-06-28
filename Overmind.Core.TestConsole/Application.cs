using Overmind.Core;
using Overmind.Core.Provider;
using Overmind.Core.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Overmind.Core.TestConsole
{
	public class Application : Shell
	{
		public Application()
		{
			dataProvider = new FileDataProvider<string, string>("DataStore", "TestData");

			commandInterpreter.RegisterCommand("data", arguments => Write(reflectionComparer.ToFullString(commandInterpreter.Invoke(dataProvider, arguments))));
		}

		private readonly ReflectionComparer reflectionComparer = new ReflectionComparer();
		private readonly IDataProvider<string, string> dataProvider;
	}
}
