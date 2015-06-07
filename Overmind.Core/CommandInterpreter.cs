using System;
using System.Collections.Generic;
using System.Linq;

namespace Overmind.Core
{
	public abstract class CommandInterpreter
	{
		private readonly IDictionary<string, Command<string[]>> commandActions;

		protected CommandInterpreter()
		{
			commandActions = new Dictionary<string, Command<string[]>>();

			RegisterCommand("help", _ => Help());
		}

		protected void RegisterCommand(string commandName, Action<string[]> execute, Predicate<string[]> canExecute = null)
		{
			commandActions.Add(commandName, new Command<string[]>(execute, canExecute));
		}

		public char[] Separators = { ' ' };

		protected string[] SplitArguments(string arguments)
		{
			return arguments.Split(Separators, StringSplitOptions.RemoveEmptyEntries);
		}

		public void ExecuteCommand(string[] commandText)
		{
			if (commandText.Any() == false)
				Help();
			else if (commandActions.ContainsKey(commandText[0]) == false)
				throw new Exception("Unknown command: " + commandText[0]);
			else
			{
				Command<string[]> command = commandActions[commandText[0]];
				string[] arguments = commandText.Skip(1).ToArray();
				if (command.CanExecute(arguments) == false)
					throw new Exception("Cannot execute command: " + commandText[0]);
				else
					command.Execute(arguments);
			}
		}

		protected virtual void Help()
		{
			Console.WriteLine(String.Join(" ", commandActions.Keys.ToArray()));
		}
	}
}