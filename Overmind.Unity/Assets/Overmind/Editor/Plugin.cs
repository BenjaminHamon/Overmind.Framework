using System;

namespace Overmind.Unity.Editor
{
	/// <summary>Plugin descriptor.</summary>
	[Serializable]
	public class Plugin
	{
		public string Name;

		public override string ToString()
		{
			return "Plugin " + Name;
		}
	}
}
