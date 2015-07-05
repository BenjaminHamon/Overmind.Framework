using UnityEngine;

namespace Overmind.Unity
{
	/// <summary>
	/// Base class for Unity scripts.
	/// </summary>
	public abstract class MonoBehaviourBase : MonoBehaviour
	{
		/// <summary>Awake is called when the script instance is being loaded.</summary>
		public virtual void Awake() { }
		/// <summary>Start is called on the frame when a script is enabled just before any of the Update methods is called the first time.</summary>
		public virtual void Start() { }

		/// <summary>This function is called when the object becomes enabled and active.</summary>
		public virtual void OnEnable() { }
		/// <summary>This function is called when the behaviour becomes disabled () or inactive.</summary>
		public virtual void OnDisable() { }

		/// <summary>Update is called every frame, if the MonoBehaviour is enabled.</summary>
		public virtual void Update() { }
		/// <summary>This function is called every fixed framerate frame, if the MonoBehaviour is enabled.</summary>
		public virtual void FixedUpdate() { }

		/// <summary>This function is called when the MonoBehaviour will be destroyed.</summary>
		public virtual void OnDestroy() { }
	}
}
