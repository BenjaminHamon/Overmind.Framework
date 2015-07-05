using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Overmind.Unity.Editor
{
	/// <summary>
	/// Static class for managing plugins in the Unity Editor.
	/// </summary>
	public static class PluginManager
	{
		/// <summary>
		/// Selects the plugin manifest in the Unity Editor. Creates one if necessary.
		/// </summary>
		[MenuItem("Overmind/Plugins/Edit manifest")]
		public static void EditManifest()
		{
			PluginManifest manifest = LoadManifest();
			Selection.activeObject = manifest;
		}

		/// <summary>
		/// <para>Imports all plugins described in the manifest to the plugin directory.</para>
		/// <para>Looks through the repositories one by one for the required files</para>
		/// </summary>
		/// <exception cref="Exception">Thrown if a plugin could not be found.</exception>
		[MenuItem("Overmind/Plugins/Reimport All")]
		public static void ReimportAll()
		{
			PluginManifest manifest = LoadManifest();
			string pluginDirectory = Path.Combine(Application.dataPath, "Plugins");
			foreach (Plugin plugin in manifest.Plugins)
			{
				bool found = false;
				foreach (string repository in manifest.Repositories)
				{
					string source = Path.Combine(repository, plugin.Name);
					string destination = Path.Combine(pluginDirectory, plugin.Name);
					if (File.Exists(source + ".dll"))
					{
						Debug.Log(String.Format("[PluginManager.ReimportAll] {0} copied from {1}", plugin, repository));
						File.Copy(source + ".dll", destination + ".dll", true);
						if (File.Exists(source + ".pdb"))
							File.Copy(source + ".pdb", destination + ".pdb", true);
						found = true;
						break;
					}
				}
				if (found == false)
					throw new Exception(String.Format("[PluginManager.ReimportAll] {0} not found", plugin));
			}
			AssetDatabase.Refresh();
		}

		private static PluginManifest LoadManifest()
		{
			PluginManifest manifest = AssetDatabase.LoadAssetAtPath<PluginManifest>("Assets/Plugins/Editor/PluginManifest.asset");
			if (manifest == null)
			{
				if (AssetDatabase.IsValidFolder("Assets/Plugins") == false)
					AssetDatabase.CreateFolder("Assets", "Plugins");
				if (AssetDatabase.IsValidFolder("Assets/Plugins/Editor") == false)
					AssetDatabase.CreateFolder("Assets/Plugins", "Editor");
				manifest = ScriptableObject.CreateInstance<PluginManifest>();
				AssetDatabase.CreateAsset(manifest, "Assets/Plugins/Editor/PluginManifest.asset");
				AssetDatabase.SaveAssets();
			}
			return manifest;
		}
	}
}
