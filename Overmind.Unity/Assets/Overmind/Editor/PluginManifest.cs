using System.Collections.Generic;
using UnityEngine;

namespace Overmind.Unity.Editor
{
	/// <summary>
	/// Stores information about plugins in a Unity project, so that they can be restored or updated.
	/// </summary>
	public class PluginManifest : ScriptableObject
	{
		/// <summary>
		/// The collection of paths where we look for plugin files.
		/// </summary>
		public List<string> Repositories;

		/// <summary>
		/// The collection of plugin descriptors.
		/// </summary>
		public List<Plugin> Plugins;
	}
}
