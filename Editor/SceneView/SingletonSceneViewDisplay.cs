﻿using Stratus.Editor;

using UnityEngine;

namespace Stratus.Unity.Editor
{
	/// <summary>
	/// A display for singletons
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public abstract class SingletonSceneViewDisplay<T> : LayoutSceneViewDisplay where T : StratusSingletonBehaviour<T>
	{
		protected virtual bool showInPlayMode { get; } = true;
		protected T instance => StratusSingletonBehaviour<T>.instance;
		protected override bool isValid => showInPlayMode && StratusSingletonBehaviour<T>.instantiated && instance.isActiveAndEnabled;
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
