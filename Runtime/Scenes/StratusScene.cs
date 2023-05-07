using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

using Stratus.Utilities;
using Stratus.Unity.Events;

using Event = Stratus.Events.Event;

#if UNITY_EDITOR
using UnityEditor.SceneManagement;
#endif

namespace Stratus.Unity.Scenes
{
	/// <summary>
	/// A space for scene-specific events, accessible to all objects
	/// </summary>  
	[StratusSingleton("Stratus Scene Events", true, true)]
	public class StratusScene : StratusSingletonBehaviour<StratusScene>
	{
		//----------------------------------------------------------------------------------/
		// Declarations
		//----------------------------------------------------------------------------------/
		public abstract class StatusEvent : Event { public string name; }

		public static void Connect<T>()
		{
			throw new NotImplementedException();
		}

		public class ChangedEvent : StatusEvent { }
		public class LoadedEvent : StatusEvent { }
		public class UnloadedEvent : StatusEvent { }
		public class ReloadEvent : Event { }

		/// <summary>
		/// Callback for scene events
		/// </summary>
		public delegate void SceneCallback();

		//----------------------------------------------------------------------------------/
		// Properties: Public
		//----------------------------------------------------------------------------------/
		/// <summary>
		/// Whether the scene (and thus the editor) is currently being edited
		/// </summary>
		public static bool isEditMode => Application.isEditor && !Application.isPlaying;

		/// <summary>    
		/// The currently active scene is the scene which will be used as the target for new GameObjects instantiated by scripts.
		/// </summary>
		public static SceneField activeScene
		{
			get
			{
#if UNITY_EDITOR
				if (isEditMode)
					return new SceneField(SceneManager.GetActiveScene().name);
#endif

				return new SceneField(SceneManager.GetActiveScene().name);
			}
		}

		/// <summary>
		/// Returns a list of all active scenes
		/// </summary>
		public static SceneField[] activeScenes
		{
			get
			{
				var scenes = new SceneField[sceneCount];
				for (var i = 0; i < sceneCount; ++i)
				{
					scenes[i] = new SceneField(GetSceneAt(i).name);
				}
				return scenes;
			}
		}

		/// <summary>
		/// The current progress into loading the next scene, on a (0,1) range
		/// </summary>
		public static float loadingProgress { get; private set; }

		/// <summary>
		/// A provided callback for when the scene has changed. Add your methods here to be
		/// notified.
		/// </summary>
		public static UnityAction onSceneChanged { get; set; }

		/// <summary>
		/// A provided callback for when all scenes in a loading operation have finished loading
		/// </summary>
		public static UnityAction onAllScenesLoaded { get; set; }

		/// <summary>
		/// A provided callback for when any scene has been loaded
		/// </summary>
		public static UnityAction onSceneLoaded { get; set; } = () => { };

		/// <summary>
		/// A provided callback for when any scene has been unloaded
		/// </summary>
		public static UnityAction onSceneUnloaded { get; set; } = () => { };

		//----------------------------------------------------------------------------------/
		// Properties: Private
		//----------------------------------------------------------------------------------/
		private static SceneCallback onSceneLoadedCallback { get; set; }
		private static SceneCallback onSceneUnloadedCallback { get; set; }
		private static SceneCallback onAllScenesLoadedCallback { get; set; }
		/// <summary>
		/// The current asynchronous loading operation
		/// </summary>
		private AsyncOperation loadingOperation { get; set; }

		private static Func<Scene> getActiveScene
		{
			get
			{
#if UNITY_EDITOR
				if (isEditMode)
					return SceneManager.GetActiveScene;
#endif
				return SceneManager.GetActiveScene;
			}
		}

		/// <summary>
		/// Keeps track of how many active scenes there are
		/// </summary>
		private static int sceneCount
		{
			get
			{
#if UNITY_EDITOR
				if (isEditMode)
					return SceneManager.sceneCount;
#endif
				return SceneManager.sceneCount;

			}
		}

		private static Scene GetSceneAt(int index)
		{
#if UNITY_EDITOR
			if (isEditMode)
				return SceneManager.GetSceneAt(index);
#endif
			return SceneManager.GetSceneAt(index);
		}

		//private static Dictionary<SceneField, SceneCallback> 


		//----------------------------------------------------------------------------------/
		// Messages
		//----------------------------------------------------------------------------------/
		protected override void OnAwake()
		{
			DontDestroyOnLoad(this.gameObject);

			onSceneChanged = new UnityAction(OnActiveSceneChanged);

			SceneManager.activeSceneChanged += OnActiveSceneChanged;
			SceneManager.sceneLoaded += OnSceneLoaded;
			SceneManager.sceneUnloaded += OnSceneUnloaded;
		}

		//----------------------------------------------------------------------------------/
		// Methods: Static
		//----------------------------------------------------------------------------------/
		/// <summary>
		/// Loads the scene specified by name.
		/// </summary>
		/// <param name="sceneName"></param>
		public static void Load(SceneField scene, LoadSceneMode mode = LoadSceneMode.Additive, SceneCallback onSceneLoaded = null)
		{
			// Edit mode
#if UNITY_EDITOR
			if (isEditMode)
			{
				EditorSceneManager.OpenScene(scene.path, OpenSceneMode.Additive);
				return;
			}
#endif

			// Play mode
			instance.StartCoroutine(instance.LoadAsync(scene, mode, onSceneLoaded));
		}

		/// <summary>
		/// Unloads the specified scene asynchronously
		/// </summary>
		/// <param name="sceneName"></param>
		public static void Unload(SceneField scene, SceneCallback onSceneUnloaded = null)
		{
			// Editor mode
#if UNITY_EDITOR
			if (isEditMode)
			{
				EditorSceneManager.CloseScene(scene.runtime, true);
				return;
			}
#endif

			// Play Mode
			instance.StartCoroutine(instance.UnloadAsync(scene, onSceneUnloaded));
		}

		/// <summary>
		/// Loads multiple scenes in sequence asynchronouosly (additively)
		/// </summary>
		/// <param name="scenes"></param>
		/// <param name="onSceneLoaded"></param>
		public static void Load(SceneField[] scenes, SceneCallback onScenesLoaded = null)
		{
			// Editor mode
#if UNITY_EDITOR
			if (isEditMode)
			{
				foreach (var scene in scenes)
				{
					EditorSceneManager.OpenScene(scene.path, OpenSceneMode.Additive);
					//EditorSceneManager.LoadScene(scene, LoadSceneMode.Additive);
				}
				return;
			}
#endif

			// Play mode
			instance.StartCoroutine(instance.LoadAsync(scenes, onScenesLoaded));
		}


		/// <summary>
		/// Unloads multiple scenes in sequence asynchronously
		/// </summary>
		/// <param name="scenes"></param>
		public static void Unload(SceneField[] scenes, SceneCallback onScenesUnloaded = null)
		{
			// Editor mode
#if UNITY_EDITOR
			if (isEditMode)
			{
				foreach (var scene in scenes)
				{
					StratusDebug.Log($"Closing {scene.name}");
					EditorSceneManager.CloseScene(scene.runtime, true);
				}
				return;
			}
#endif

			// Play mode
			instance.StartCoroutine(instance.UnloadAsync(scenes, onScenesUnloaded));

		}

		/// <summary>
		/// Reloads the current scene
		/// </summary>
		public static void Reload()
		{
			Load(activeScene, LoadSceneMode.Single);
		}

		/// <summary>
		/// Reloads the specified scene
		/// </summary>
		public static void Reload(SceneField scene, SceneCallback onFinished = null)
		{
			Load(activeScene, LoadSceneMode.Single);
		}

		/// <summary>
		/// Subscribes to events dispatched onto the scene
		/// </summary>
		/// <typeparam name="TEvent"></typeparam>
		/// <param name="func"></param>
		public static void Connect<TEvent>(Action<TEvent> func) where TEvent : Event
		{
			instance.Poke();
			UnityStratusEventSystem.Connect(instance.gameObject, func);
		}

		/// <summary>
		/// Subscribes to events dispatched onto the scene
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="func"></param>
		public static void Connect(Action<Event> func, Type type)
		{
			instance.Poke();
			UnityStratusEventSystem.Connect(instance.gameObject, type, func);
		}

		/// <summary>
		/// Dispatches an event to the scene
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="eventObj"></param>
		public static void Dispatch<T>(T eventObj) where T : Event
		{
			instance.Poke();
			UnityStratusEventSystem.Dispatch<T>(instance.gameObject, eventObj);
		}

		/// <summary>
		/// Dispatches the given event of the specified type onto this object.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="gameObj">The GameObject to which to connect to.</param>
		/// <param name="eventObj">The event object. </param>
		/// <param name="nextFrame">Whether the event should be sent next frame.</param>
		public static void Dispatch(Event eventObj, Type type)
		{
			instance.Poke();
			UnityStratusEventSystem.Dispatch(instance.gameObject, eventObj);
		}

		/// <summary>
		/// Finds all the components of a given type in the active scene, possibly including inactive ones.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="includeInactive"></param>
		/// <returns></returns>
		public static T[] GetComponentsInActiveScene<T>(bool includeInactive = false) where T : Component
		{
			return activeScene.GetComponents<T>(includeInactive);
		}

		/// <summary>
		/// Finds all the components of a given type in all active scenes, possibly including inactive ones.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="includeInactive"></param>
		/// <returns></returns>
		public static T[] GetComponentsInAllActiveScenes<T>(bool includeInactive = false) where T : Component
		{
			List<T> components = new List<T>();

			foreach (var scene in activeScenes)
			{
				var matchingComponents = scene.GetComponents<T>(includeInactive);
				components.AddRange(matchingComponents);
			}

			return components.ToArray();
		}


		//------------------------------------------------------------------------/
		// Methods: Private
		//------------------------------------------------------------------------/
		/// <summary>
		/// Received when the active scene changes
		/// </summary>
		/// <param name="prevScene"></param>
		/// <param name="nextScene"></param>
		private void OnActiveSceneChanged(Scene prevScene, Scene nextScene)
		{
			onSceneChanged.Invoke();
		}

		/// <summary>
		/// Received when a scene has been loaded
		/// </summary>
		/// <param name="arg0"></param>
		/// <param name="arg1"></param>
		private void OnSceneLoaded(Scene prevScene, LoadSceneMode mode)
		{
			onSceneLoaded.Invoke();
			onSceneLoadedCallback?.DynamicInvoke();
			onSceneLoadedCallback = null;
		}

		/// <summary>
		/// Received when a scene has been unloaded
		/// </summary>
		/// <param name="arg0"></param>
		private void OnSceneUnloaded(Scene scene)
		{
			onSceneLoaded.Invoke();
			onSceneUnloadedCallback?.DynamicInvoke();
			onSceneUnloadedCallback = null;
		}
		void OnActiveSceneChanged()
		{
		}


		/// <summary>
		/// Loads the specified scene asynchronously
		/// </summary>
		/// <param name="sceneName"></param>
		/// <param name="mode"></param>
		IEnumerator LoadAsync(string sceneName, LoadSceneMode mode, SceneCallback onFinished = null)
		{
			loadingProgress = 0f;
			loadingOperation = SceneManager.LoadSceneAsync(sceneName, mode);
			if (loadingOperation != null)
			{
				while (!loadingOperation.isDone)
				{
					loadingProgress = loadingOperation.progress;
					yield return null;
				}
			}

			loadingProgress = 1f;
			onFinished?.Invoke();
		}

		/// <summary>
		/// Loads the specified scene asynchronously
		/// </summary>
		/// <param name="sceneName"></param>
		/// <param name="mode"></param>
		IEnumerator UnloadAsync(string sceneName, SceneCallback onFinished = null)
		{
			loadingProgress = 0f;
			loadingOperation = SceneManager.UnloadSceneAsync(sceneName);
			if (loadingOperation != null)
			{
				while (!loadingOperation.isDone)
				{
					loadingProgress = loadingOperation.progress;
					yield return null;
				}
			}

			loadingProgress = 1f;
			onFinished?.Invoke();
		}

		/// <summary>
		/// Loads multiple scenes asynchronously in sequence, additively
		/// </summary>
		/// <param name="scenes"></param>
		/// <returns></returns>
		IEnumerator LoadAsync(SceneField[] scenes, SceneCallback onFinished = null)
		{
			loadingProgress = 0f;
			// Get the scene names in a queue
			var sceneNames = new Queue<string>();
			foreach (var scene in scenes)
				sceneNames.Enqueue(scene);

			while (sceneNames.Count > 0)
			{
				var nextScene = sceneNames.Dequeue();
				loadingOperation = SceneManager.LoadSceneAsync(nextScene, LoadSceneMode.Additive);
				while (!loadingOperation.isDone)
				{
					loadingProgress = loadingOperation.progress / scenes.Length;
					yield return null;
				}
				//Trace.Script("Finished loading " + nextScene);
			}

			loadingProgress = 1f;
			onFinished?.Invoke();
			onAllScenesLoadedCallback?.Invoke();
		}

		/// <summary>
		/// Loads multiple scenes asynchronously in sequence, additively
		/// </summary>
		/// <param name="scenes"></param>
		/// <returns></returns>
		IEnumerator UnloadAsync(SceneField[] scenes, SceneCallback onFinished = null)
		{
			loadingProgress = 0f;
			// Get the scene names in a queue
			var sceneQueue = new Queue<SceneField>();
			foreach (var scene in scenes)
				sceneQueue.Enqueue(scene);

			while (sceneQueue.Count > 0)
			{
				var nextScene = sceneQueue.Dequeue();
				if (!nextScene.isLoaded)
					continue;

				loadingOperation = SceneManager.UnloadSceneAsync(nextScene);
				while (!loadingOperation.isDone)
				{
					loadingProgress = loadingOperation.progress / scenes.Length;
					yield return null;
				}
				//Trace.Script("Finished unloading " + nextScene);
			}

			loadingProgress = 1f;
			onFinished?.Invoke();
			onAllScenesLoadedCallback?.Invoke();
		}
	}

	public static class UnityStratusEventExtensions
	{
		public static void DispatchToScene(this Event e) => StratusScene.Dispatch(e, e.GetType());
	}
}
