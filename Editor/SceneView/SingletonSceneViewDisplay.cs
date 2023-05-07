using Stratus.Editor;
using Stratus.Unity.Behaviours;

using UnityEngine;

namespace Stratus.Unity.Editor
{
	/// <summary>
	/// A display for singletons
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public abstract class SingletonSceneViewDisplay<T> : LayoutSceneViewDisplay where T : SingletonBehaviour<T>
	{
		protected virtual bool showInPlayMode { get; } = true;
		protected T instance => SingletonBehaviour<T>.instance;
		protected override bool isValid => showInPlayMode && SingletonBehaviour<T>.instantiated && instance.isActiveAndEnabled;
		protected abstract void OnInitializeSingletonState();


		protected StratusSerializedPropertyMap properties { get; private set; }

		protected override void OnInitializeState()
		{
			if (instance)
				properties = new StratusSerializedPropertyMap(instance, typeof(MonoBehaviour));
			OnInitializeSingletonState();
		}

	}
}
