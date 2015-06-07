using System;

namespace Overmind.Core
{
	public class Command<TData>
	{
		private static Predicate<TData> DefaultCanExecute = _ => true;

		public Command(Action<TData> execute, Predicate<TData> canExecute = null)
		{
			this.execute = execute;
			this.canExecute = canExecute ?? DefaultCanExecute;
		}

		private readonly Predicate<TData> canExecute;
		public Predicate<TData> CanExecute { get { return canExecute; } }

		private readonly Action<TData> execute;
		public Action<TData> Execute { get { return execute; } }
	}
}
