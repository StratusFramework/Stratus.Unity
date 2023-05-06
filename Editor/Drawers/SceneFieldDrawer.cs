using UnityEditor;

using UnityEngine;

namespace Stratus.Unity.Editor
{
	[CustomPropertyDrawer(typeof(StratusSceneField))]
	public class SceneFieldDrawer : PropertyDrawer
	{
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			SerializedProperty sceneAssetProp = property.FindPropertyRelative(nameof(StratusSceneField.sceneAsset));

			EditorGUI.BeginProperty(position, label, sceneAssetProp);
			EditorGUI.PropertyField(position, sceneAssetProp, label);
			EditorGUI.EndProperty();
		}
	}
}