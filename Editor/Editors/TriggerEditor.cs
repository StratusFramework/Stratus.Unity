using Stratus.Unity.Triggers;

using UnityEditor;

namespace Stratus.Unity.Editor
{
	[CustomEditor(typeof(TriggerBehaviour), true), CanEditMultipleObjects]
	public class TriggerEditor : TriggerBaseEditor<TriggerBehaviour>
	{
		internal override void OnTriggerBaseEditorEnable()
		{
		}
	}

	[CustomEditor(typeof(TriggerableBehaviour), true), CanEditMultipleObjects]
	public class TriggerableEditor : TriggerBaseEditor<TriggerableBehaviour>
	{
		internal override void OnTriggerBaseEditorEnable()
		{
		}
	}

	[CustomEditor(typeof(TriggerBehaviour), true), CanEditMultipleObjects]
	public abstract class TriggerEditor<T> : TriggerEditor where T : TriggerBehaviour
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

	[CustomEditor(typeof(TriggerableBehaviour), true), CanEditMultipleObjects]
	public abstract class TriggerableEditor<T> : TriggerableEditor where T : TriggerableBehaviour
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