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

			commandInterpreter.RegisterExecutor("data", dataProvider, result => Write(reflectionComparer.ToFullString(result)));
		}

		private readonly ReflectionComparer reflectionComparer = new ReflectionComparer();
		private readonly IDataProvider<string, string> dataProvider;
	}
}
