using Stratus.Unity.Triggers;

using UnityEditor;

namespace Stratus.Unity.Editor
{
	[CustomEditor(typeof(SceneTriggerable))]
	public class SceneEventEditor : TriggerableEditor<SceneTriggerable>
	{
		SceneTriggerable sceneEvent => target as SceneTriggerable;

		protected override void OnTriggerableEditorEnable()
		{
		}

		protected override bool DrawDeclaredProperties()
		{
			bool changed = false;
			changed |= DrawSerializedProperty(declaredProperties.Item2[0].unitySerialization, serializedObject);
			if (sceneEvent.type == SceneTriggerable.Type.Load || sceneEvent.type == SceneTriggerable.Type.Unload)
			{
				changed |= DrawSerializedProperty(declaredProperties.Item2[1].unitySerialization, serializedObject);
			}
			if (sceneEvent.type == SceneTriggerable.Type.Load)
			{
				changed |= DrawSerializedProperty(declaredProperties.Item2[2].unitySerialization, serializedObject);
			}
			return changed;
		}
	}
}