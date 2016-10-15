using Overmind.Framework.Core;
using Overmind.Framework.Core.Provider;
using Overmind.Framework.Core.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Overmind.Framework.TestConsole
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
