using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Stratus.Utilities;
using System.Reflection;
using System;

namespace Stratus.Editor
{
	[CustomEditor(typeof(StratusTriggerBehaviour), true), CanEditMultipleObjects]
	public class TriggerEditor : StratusTriggerBaseEditor<StratusTriggerBehaviour>
	{
		internal override void OnTriggerBaseEditorEnable()
		{
		}
	}

	[CustomEditor(typeof(StratusTriggerableBehaviour), true), CanEditMultipleObjects]
	public class TriggerableEditor : StratusTriggerBaseEditor<StratusTriggerableBehaviour>
	{
		internal override void OnTriggerBaseEditorEnable()
		{
		}
	}

	[CustomEditor(typeof(StratusTriggerBehaviour), true), CanEditMultipleObjects]
	public abstract class TriggerEditor<T> : TriggerEditor where T : StratusTriggerBehaviour
	{
		/// <summary>
		/// The target cast as the declared trigger type
		/// </summary>
		protected T trigger { get; private set; }

		protected abstract void OnTriggerEditorEnable();

		internal override void OnTriggerBaseEditorEnable()
		{
			trigger = base.target as T;
			OnTriggerEditorEnable();
		}
	}

	[CustomEditor(typeof(StratusTriggerableBehaviour), true), CanEditMultipleObjects]
	public abstract class TriggerableEditor<T> : TriggerableEditor where T : StratusTriggerableBehaviour
	{
		/// <summary>
		/// The target cast as the declared triggerable type
		/// </summary>
		protected T triggerable { get; private set; }

		protected abstract void OnTriggerableEditorEnable();

		internal override void OnTriggerBaseEditorEnable()
		{
			triggerable = base.target as T;
			OnTriggerableEditorEnable();
		}
	}



}