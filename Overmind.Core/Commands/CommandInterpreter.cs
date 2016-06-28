using Overmind.Core.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Overmind.Core.Commands
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
			// TODO: Handle quotes
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
						if (command.CanExecute(arguments) == false)
							throw new Exception("[CommandInterpreter.ExecuteCommand] Cannot execute command: " + commandText);
						else
							command.Execute(arguments);
					}
				}
			}
		}

		/// <summary>Invokes a command on an object by matching the command to a method using reflection.</summary>
		/// <typeparam name="TObject">The object type on which to search the method. This can be used to pass an interface or a base type.</typeparam>
		/// <param name="obj">The object on which to invoke the method.</param>
		/// <param name="arguments">The command argument list.</param>
		/// <returns>The value returned by the invoked method.</returns>
		/// <exception cref="AmbiguousMatchException">Thrown if more than one method matches the command.</exception>
		/// <exception cref="IndexOutOfRangeException">Thrown if there are not enough arguments to invoke the method.</exception>
		public object Invoke<TObject>(TObject obj, IList<string> arguments)
		{
			MethodInfo method = method = typeof(TObject).GetMethod(arguments[1], BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
			IList<ParameterInfo> parameters = method.GetParameters();
			object[] parameterValues = new object[parameters.Count];
			const int argumentOffset = 2; // To ignore the command and method names from the arguments

			for (int parameterIndex = 0; parameterIndex < parameters.Count; parameterIndex++)
			{
				// Tries to convert the argument strings to the types expected by the method.
				// This can become tricky, for example Redis mset command expects a list of keys and a list of values, alternating keys and values.
				object value = null;
				if (parameters[parameterIndex].ParameterType == typeof(byte[]))
					value = ByteExtensions.FromHexString(arguments[parameterIndex + argumentOffset]);
				else if (parameters[parameterIndex].ParameterType == typeof(string[]))
					value = arguments.Skip(argumentOffset).ToArray();
				else
					value = Convert.ChangeType(arguments[parameterIndex + argumentOffset], parameters[parameterIndex].ParameterType);
				parameterValues[parameterIndex] = value;
			}

			return method.Invoke(obj, parameterValues);
		}
	}
}