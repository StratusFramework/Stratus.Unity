using UnityEditor;
using Stratus.Unity.Triggers;

namespace Stratus.Unity.Editor
{
	[CustomEditor(typeof(CompositeTrigger))]
	public class CompositeTriggerEditor : TriggerEditor<CompositeTrigger>
	{
		protected override void OnTriggerEditorEnable()
		{
			propertyConstraints.Add(propertyMap["triggers"], () => trigger.type == CompositeTrigger.Type.Trigger);
			propertyConstraints.Add(propertyMap["triggerables"], () => trigger.type == CompositeTrigger.Type.Triggerable);
		}

		protected override bool DrawDeclaredProperties()
		{
			bool changed = false;

			EditorGUI.BeginChangeCheck();
			DrawSerializedProperty(nameof(CompositeTrigger.type));
			DrawSerializedProperty(nameof(CompositeTrigger.criteria));

			if (trigger.criteria == CompositeTrigger.Criteria.Subset)
				trigger.needed = EditorGUILayout.IntSlider(trigger.needed, 1, trigger.count);

			if (trigger.type == CompositeTrigger.Type.Trigger)
			{
				DrawSerializedProperty(nameof(CompositeTrigger.triggers));
			}
			else if (trigger.type == CompositeTrigger.Type.Triggerable)
			{
				DrawSerializedProperty(nameof(CompositeTrigger.triggerables));
			}
			changed = EditorGUI.EndChangeCheck();
			return changed;
		}
	}
}