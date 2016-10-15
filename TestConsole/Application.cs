using Overmind.Framework.Core;
using Overmind.Framework.Core.Provider;
using Overmind.Framework.Core.Reflection;
using System;
using System.Collections.Generic;

namespace Overmind.Framework.TestConsole
{
	public class Application : Shell
	{
		public Application()
		{
			dataProvider = new FileDataProvider<string, string>("DataStore", "TestData");
			testExecutor = new TestExecutor(this);
		}

		private readonly ReflectionComparer reflectionComparer = new ReflectionComparer();
		private readonly IDataProvider<string, string> dataProvider;
		private readonly TestExecutor testExecutor;

		protected override void Initialize(IList<string> arguments)
		{
			base.Initialize(arguments);

			Console.Title = "Overmind.Core.TestConsole";

			commandInterpreter.RegisterExecutor("data", dataProvider, result => Write(reflectionComparer.ToFullString(result)));
			commandInterpreter.RegisterExecutor("test", testExecutor);
		}
	}
}
