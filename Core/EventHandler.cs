namespace Overmind.Core
{
	// We reimplement EventHandler instead of using System.Action or System.EventHandler in order to:
	// - have meaningful parameter names like sender instead of arg1, arg2
	// - have a generic type parameter for sender instead of System.Object
	// - not require the eventArgs parameter to inherit System.EventArgs

	/// <summary>Generic delegate type for a method will handle an event when raised.</summary>
	/// <typeparam name="TSender">The type of the objet raising the event.</typeparam>
	/// <param name="sender">The object raising the event.</param>
	/// <remarks>This implementation differs from System.EventHandler in specifying the sender type instead of using System.Object.</remarks>
	public delegate void EventHandler<TSender>(TSender sender);

	/// <summary>Generic delegate type for a method will handle an event when raised.</summary>
	/// <typeparam name="TSender">The type of the objet raising the event.</typeparam>
	/// <typeparam name="TEventArgs">The type of the data sent with the event.</typeparam>
	/// <param name="sender">The object raising the event.</param>
	/// <param name="eventArgs">The data sent with the event, should be immutable.</param>
	/// <remarks>This implementation differs from System.EventHandler in specifying the sender type instead of using System.Object
	/// and in not requiring the data type to inherit System.EventArgs.</remarks>
	public delegate void EventHandler<TSender, TEventArgs>(TSender sender, TEventArgs eventArgs);
}
