using Stratus.Unity.Triggers;

using UnityEditor;

namespace Stratus.Unity.Editor
{
	[CustomEditor(typeof(LogTriggerable))]
	public class LogEventEditor : TriggerableEditor<LogTriggerable>
	{
		protected override void OnTriggerableEditorEnable()
		{
			SerializedProperty descriptionProperty = propertyMap[nameof(TriggerBase.description)];
			propertyDrawOverrides.Remove(descriptionProperty);
		}
	}
}