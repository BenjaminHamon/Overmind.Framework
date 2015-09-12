using System;
using System.Collections.Generic;
using System.Linq;

namespace Overmind.Core
{
	public class CommandInterpreter
	{
		private readonly IDictionary<string, Command<IList<string>>> commandCollection = new Dictionary<string, Command<IList<string>>>();
		public IEnumerable<string> CommandNames { get { return commandCollection.Keys; } }

		public void RegisterCommand(string name, Action<IList<string>> execute, Predicate<IList<string>> canExecute = null)
		{
			commandCollection.Add(name, new Command<IList<string>>(execute, canExecute));
		}

		public char[] Separators = { ' ' };

		protected IList<string> SplitArguments(string arguments)
		{
			return arguments.Split(Separators, StringSplitOptions.RemoveEmptyEntries);
		}

		public void ExecuteCommand(string commandText)
		{
			if (String.IsNullOrEmpty(commandText) == false)
			{
				IList<string> arguments = SplitArguments(commandText);
				if (arguments.Any())
				{
					string commandName = arguments.First();
					if (commandCollection.ContainsKey(commandName) == false)
						throw new Exception("[CommandInterpreter.ExecuteCommand] Unknown command: " + commandName);
					else
					{
						Command<IList<string>> command = commandCollection[commandName];
						arguments = arguments.Skip(1).ToList();
						if (command.CanExecute(arguments) == false)
							throw new Exception("[CommandInterpreter.ExecuteCommand] Cannot execute command: " + commandText);
						else
							command.Execute(arguments);
					}
				}
			}
		}
	}
}