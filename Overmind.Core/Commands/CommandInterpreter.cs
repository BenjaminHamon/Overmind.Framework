using Overmind.Core.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Overmind.Core.Commands
{
	/// <summary>
	/// Parses and executes string based commands.
	/// </summary>
	/// <remarks>
	/// <para>About executors:</para>
	/// <para>An executor is a special command associated with an object.
	/// The interpreter uses reflection to map arguments passed to an executor command to a public method in the executor object.
	/// This is useful to quickly create a powerful command to make calls to any object interface.</para>
	/// </remarks>
	public class CommandInterpreter
	{
		private readonly IDictionary<string, Command<IList<string>>> commandCollection = new Dictionary<string, Command<IList<string>>>();
		public IEnumerable<string> CommandNames { get { return commandCollection.Keys; } }

		private readonly IDictionary<string, Type> executorTypeCollection = new Dictionary<string, Type>();

		public void RegisterCommand(string name, Action<IList<string>> execute, Predicate<IList<string>> canExecute = null)
		{
			commandCollection.Add(name, new Command<IList<string>>(execute, canExecute));
		}

		public void RegisterExecutor<TExecutor>(string executorName, TExecutor executor, Action<object> resultHandler = null)
		{
			Action<IList<string>> commandAction = arguments =>
			{
				object result = Invoke<TExecutor>(executor, arguments);
				if (resultHandler != null)
					resultHandler.Invoke(result);
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
						throw new OvermindException("[CommandInterpreter] Unknown command: " + commandName);
					else
					{
						Command<IList<string>> command = commandCollection[commandName];
						if (command.CanExecute(arguments) == false)
							throw new OvermindException("[CommandInterpreter] Cannot execute command: " + commandText);
						else
							command.Execute(arguments);
					}
				}
			}
		}

		/// <summary>Invokes a command on an object by matching the command to a method using reflection.</summary>
		/// <typeparam name="TObject">The object type on which to search the method. This can be used to pass an interface or a base type.</typeparam>
		/// <param name="executor">The object on which to invoke the method.</param>
		/// <param name="argumentCollection">The command argument list.</param>
		/// <returns>The value returned by the invoked method.</returns>
		/// <exception cref="OvermindException">Thrown if the command could not be matched to an executor method.</exception>
		public object Invoke<TObject>(TObject executor, IList<string> argumentCollection)
		{
			const int argumentOffset = 2; // To ignore the executor and method names from the arguments

			if (argumentCollection.Count < 2)
				throw new OvermindException("[CommandInterpreter] Invalid arguments: missing executor method name");

			int methodArgumentCount = argumentCollection.Count - argumentOffset;
			MethodInfo methodInfo = GetExecutorMethods(typeof(TObject))
				.SingleOrDefault(method => method.Name.Equals(argumentCollection[1], StringComparison.InvariantCultureIgnoreCase)
					&& methodArgumentCount <= method.GetParameters().Length
					&& methodArgumentCount >= method.GetParameters().Count(p => p.IsOptional == false));
			if (methodInfo == null)
				throw new OvermindException("[CommandInterpreter] Invalid arguments: executor method not found");

			IList<ParameterInfo> parameters = methodInfo.GetParameters();
			object[] parameterValues = new object[parameters.Count];

			for (int parameterIndex = 0; parameterIndex < methodArgumentCount; parameterIndex++)
			{
				object value = null;
				Type parameterType = parameters[parameterIndex].ParameterType;
				string argument = argumentCollection[parameterIndex + argumentOffset];

				if (parameterType == typeof(byte[]))
					value = ByteExtensions.FromHexString(argument);
				else if (parameterType.IsEnum)
					value = Enum.Parse(parameterType, argument, true);
				else
					value = Convert.ChangeType(argument, parameterType);
				parameterValues[parameterIndex] = value;
			}

			for (int parameterIndex = methodArgumentCount; parameterIndex < parameters.Count; parameterIndex++)
				parameterValues[parameterIndex] = parameters[parameterIndex].DefaultValue;

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
			IEnumerable<MethodInfo> methodCollection = GetExecutorMethods(executorType);
			if (String.IsNullOrEmpty(methodName) == false)
				methodCollection = methodCollection.Where(method => method.Name.Equals(methodName, StringComparison.InvariantCultureIgnoreCase));

			foreach (MethodInfo method in methodCollection)
			{
				IEnumerable<string> parameterCollection = method.GetParameters()
					.Select(parameter => parameter.ParameterType.Name + " " + parameter.Name + (parameter.IsOptional ? " = " + parameter.DefaultValue : ""));
				descriptionBuilder.AppendLine(method.Name + "(" + String.Join(", ", parameterCollection.ToArray()) + ")");
			}

			return descriptionBuilder.ToString();
		}

		private IEnumerable<MethodInfo> GetExecutorMethods(Type executorType)
		{
			return executorType.GetMethods(BindingFlags.Public | BindingFlags.Instance)
				.Where(method => method.DeclaringType != typeof(object));
		}
	}
}