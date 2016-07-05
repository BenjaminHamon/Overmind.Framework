using Overmind.Core.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Overmind.Core.Commands
{
	public class CommandInterpreter
	{
		private readonly IDictionary<string, Command<IList<string>>> commandCollection = new Dictionary<string, Command<IList<string>>>();
		public IEnumerable<string> CommandNames { get { return commandCollection.Keys; } }

		private readonly IDictionary<string, Type> executorTypeCollection = new Dictionary<string, Type>();
		private readonly BindingFlags executorBindingFlags = BindingFlags.Public | BindingFlags.Instance;

		public void RegisterCommand(string name, Action<IList<string>> execute, Predicate<IList<string>> canExecute = null)
		{
			commandCollection.Add(name, new Command<IList<string>>(execute, canExecute));
		}

		public void RegisterExecutor<TExecutor>(string executorName, TExecutor executor, Action<object> resultHandler = null)
		{
			Action<IList<string>> commandAction = arguments =>
			{
				object result = Invoke<TExecutor>(executor, arguments);
				resultHandler?.Invoke(result);
			};
			commandCollection.Add(executorName, new Command<IList<string>>(commandAction));
			executorTypeCollection.Add(executorName, typeof(TExecutor));
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
		/// <param name="executor">The object on which to invoke the method.</param>
		/// <param name="arguments">The command argument list.</param>
		/// <returns>The value returned by the invoked method.</returns>
		/// <exception cref="AmbiguousMatchException">Thrown if more than one method matches the command.</exception>
		/// <exception cref="IndexOutOfRangeException">Thrown if there are not enough arguments to invoke the method.</exception>
		public object Invoke<TObject>(TObject executor, IList<string> arguments)
		{
			const int argumentOffset = 2; // To ignore the executor and method names from the arguments

			MethodInfo methodInfo = typeof(TObject).GetMethods(executorBindingFlags)
				.First(method => method.Name.Equals(arguments[1], StringComparison.InvariantCultureIgnoreCase)
					&& method.GetParameters().Length == arguments.Count - argumentOffset);

			IList<ParameterInfo> parameters = methodInfo.GetParameters();
			object[] parameterValues = new object[parameters.Count];

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

			return methodInfo.Invoke(executor, parameterValues);
		}

		/// <summary>Returns a description for the commands which can be invoked with a previously registered executor.</summary>
		/// <param name="executorName">The executor name.</param>
		/// <param name="methodName">Optional method name to filter the results.</param>
		/// <returns>The executor commands description.</returns>
		public string DescribeInvoke(string executorName, string methodName = null)
		{
			StringBuilder descriptionBuilder = new StringBuilder();
			Type executorType = executorTypeCollection[executorName];
			IEnumerable<MethodInfo> methodCollection = executorType.GetMethods(executorBindingFlags);
			if (String.IsNullOrEmpty(methodName) == false)
				methodCollection = methodCollection.Where(method => method.Name.Equals(methodName, StringComparison.InvariantCultureIgnoreCase));

			foreach (MethodInfo method in methodCollection)
			{
				IEnumerable<string> parameterCollection = method.GetParameters().Select(parameter => parameter.ParameterType.Name + " " + parameter.Name);
				descriptionBuilder.AppendLine(method.Name + "("+ String.Join(", ", parameterCollection.ToArray()) + ")");
			}

			return descriptionBuilder.ToString();
		}
	}
}