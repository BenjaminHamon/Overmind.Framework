using Overmind.Core.Commands;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Overmind.Core
{
	/// <summary>
	/// Base class for a command interpreter user interface.
	/// It reads commands from an input, parses and executes them.
	/// </summary>
	public abstract class Shell
	{
		protected Shell()
		{
			commandInterpreter.RegisterCommand("help", Help);
			commandInterpreter.RegisterCommand("exit", _ => Exit());
		}

		#region Execution

		protected readonly CommandInterpreter commandInterpreter = new CommandInterpreter();

		private bool isRunning = false;

		public string Prompt = "> ";
		public string Separator = Environment.NewLine;

		/// <summary>Initializes members according to the shell invocation arguments, before the main loop starts.</summary>
		/// <param name="arguments">Argument line provided with the shell invocation in the parent program or shell.</param>
		protected virtual void Initialize(IList<string> arguments) { }

		/// <summary>Starts and runs the shell, waiting for commands and executing them, until it exits.</summary>
		/// <param name="arguments">Argument line provided with the shell invocation in the parent program or shell.</param>
		public void Run(IList<string> arguments)
		{
			isRunning = true;
			Initialize(arguments);

			while (isRunning)
			{
				Output.Write(Prompt);

				try
				{
					commandInterpreter.ExecuteCommand(Input.ReadLine());
				}
				catch (Exception exception)
				{
					if (exception is TargetInvocationException)
						exception = exception.InnerException;
					WriteError(exception);
				}

				Output.Write(Separator);
			}
		}

		#endregion // Execution

		#region Input and output

		public TextReader Input = System.Console.In;
		public TextWriter Output = System.Console.Out;
		public TextWriter ErrorOutput = System.Console.Error;

		public bool StackTraceEnabled = true;

		/// <summary>Helper to read input with a message explaining what is expected.</summary>
		/// <param name="message">Optional message explaining what kind of input is expected.</param>
		/// <returns>The line read from the input reader.</returns>
		public string Read(string message = null)
		{
			if (String.IsNullOrEmpty(message) == false)
				Output.Write(message + ": ");
			return Input.ReadLine();
		}

		public void Write(string message)
		{
			Output.WriteLine(message);
		}

		public void WriteError(string message)
		{
			ConsoleColor oldColor = System.Console.ForegroundColor;
			System.Console.ForegroundColor = ConsoleColor.Red;
			ErrorOutput.WriteLine(message);
			System.Console.ForegroundColor = oldColor;
		}

		public void WriteError(Exception exception)
		{
			ConsoleColor oldColor = System.Console.ForegroundColor;
			System.Console.ForegroundColor = ConsoleColor.Red;
			if (StackTraceEnabled)
				ErrorOutput.WriteLine(exception);
			else
				ErrorOutput.WriteLine(exception.Message);
			System.Console.ForegroundColor = oldColor;
		}

		#endregion // Input and output

		#region Commands

		/// <summary>Disposes resources and stops the shell execution.</summary>
		protected virtual void Exit()
		{
			isRunning = false;
		}

		/// <summary>Handler for the help command. Displays the list of available commands.</summary>
		/// <param name="arguments"></param>
		protected virtual void Help(IList<string> arguments)
		{
			switch (arguments.Count)
			{
				case 1: Output.WriteLine(String.Join(" ", commandInterpreter.CommandNames.OrderBy(c => c).ToArray())); break;
				case 2: Output.Write(commandInterpreter.DescribeInvoke(arguments[1])); break;
				case 3: Output.Write(commandInterpreter.DescribeInvoke(arguments[1], arguments[2])); break;
				default: throw new Exception("Invalid arguments");
			}
		}

		#endregion // Commands
	}
}