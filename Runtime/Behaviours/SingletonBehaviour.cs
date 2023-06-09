using Stratus.Extensions;
using Stratus.Reflection;
using Stratus.Unity.Utility;
using Stratus.Utilities;

using UnityEngine;

namespace Stratus.Unity.Behaviours
{
	/// <summary>
	/// A singleton is a class with only one active instance, instantiated if not present when its
	/// static members are accessed. Use the [Singleton] attribute on the class 
	/// declaration in order to override the default settings.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	[DisallowMultipleComponent]
	public abstract class SingletonBehaviour<T> : StratusBehaviour where T : StratusBehaviour
	{
		//------------------------------------------------------------------------/
		// Properties
		//------------------------------------------------------------------------/
		/// <summary>
		/// Whether this singleton will be persistent whenever its parent scene is destroyed
		/// </summary>
		protected static bool isPersistent => attribute?.GetProperty<bool>(nameof(StratusSingletonAttribute.persistent)) ?? true;
		/// <summary>
		/// Whether the application is currently quitting play mode
		/// </summary>
		protected static bool isQuitting { get; set; } = false;
		/// <summary>
		/// Returns the current specific attributes for the derived singleton class, if any are present
		/// </summary>
		private static StratusSingletonAttribute attribute => typeof(T).GetAttribute<StratusSingletonAttribute>();
		/// <summary>
		/// Whether the class should be instantiated. By default, true.
		/// </summary>
		private static bool shouldInstantiate => attribute?.GetProperty<bool>(nameof(StratusSingletonAttribute.instantiate)) ?? true;
		/// <summary>
		/// Whether the class should be instantiated while in editor mode
		/// </summary>
		private static bool isPlayerOnly => attribute?.GetProperty<bool>(nameof(StratusSingletonAttribute.isPlayerOnly)) ?? true;
		/// <summary>
		/// What name to use for GameObject this singleton will be instantiated on
		/// </summary>
		private static string ownerName => attribute?.GetProperty<string>(nameof(StratusSingletonAttribute.name)) ?? typeof(T).Name;
		/// <summary>
		/// Returns a reference to the singular instance of this class. If not available currently, 
		/// it will instantiate it when accessed.
		/// </summary>
		public static T instance
		{
			get
			{
				// Look for an instance in the scene
				if (!_instance)
				{
					_instance = FindObjectOfType<T>();

					// If not found, instantiate
					if (!_instance)
					{
						if (shouldInstantiate == false || isPlayerOnly && EditorBridge.isEditMode)
						{
							//Trace.Script("Will not instantiate the class " + typeof(T).Name);
							return null;
						}

						//Trace.Script("Creating " + typeof(T).Name);
						GameObject obj = new GameObject
						{
							name = ownerName
						};
						_instance = obj.AddComponent<T>();
					}
				}

				return _instance;
			}
		}
		/// <summary>
		/// Whether this singleton has been instantiated
		/// </summary>
		public static bool instantiated => instance != null;
		//------------------------------------------------------------------------/
		// Fields
		//------------------------------------------------------------------------/
		/// <summary>
		/// The singular instance of the class
		/// </summary>
		protected static T _instance;

		//------------------------------------------------------------------------/
		// Interface
		//------------------------------------------------------------------------/
		protected abstract void OnAwake();
		protected virtual void OnSingletonDestroyed() { }

		//------------------------------------------------------------------------/
		// Messages
		//------------------------------------------------------------------------/
		private void Awake()
		{
			// If the singleton instance hasn't been set, set it to self
			if (!instance)
			{
				_instance = this as T;
			}

			// If we are the singleton instance that was created (or recently set)
			if (instance == this as T)
			{
				if (isPersistent)
				{
					if (!EditorBridge.isEditMode)
					{
						this.OnDontDestroyOnLoad();
					}
				}

				this.OnAwake();
			}
			// If we are not...
			else
			{
				Destroy(this.gameObject);
			}
		}

		private void OnDestroy()
		{
			if (this != _instance)
			{
				return;
			}

			_instance = null;
			this.OnSingletonDestroyed();
		}

		private void OnApplicationQuit()
		{
			isQuitting = true;
		}

		protected virtual void OnDontDestroyOnLoad()
		{
			this.transform.SetParent(null);
			DontDestroyOnLoad(this);
		}

		//------------------------------------------------------------------------/
		// Methods
		//------------------------------------------------------------------------/
		/// <summary>
		/// Instantiates this singleton if possible
		/// </summary>
		/// <returns></returns>
		public static bool Instantiate()
		{
			if (instance != null)
			{
				return true;
			}
			return false;
		}

		protected void Poke()
		{
		}


	}


}