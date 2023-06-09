using Stratus.Unity.Data;

using UnityEditor;

using UnityEngine;

namespace Stratus.Unity.Editor
{
	[CustomPropertyDrawer(typeof(PositionField))]
	public class PositionFieldDrawer : PropertyDrawer
	{
		float typeWidth { get; } = 0.3f;
		float inputValueWidth { get { return 1f - typeWidth; } }


		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			return EditorGUIUtility.singleLineHeight;
		}

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			label = EditorGUI.BeginProperty(position, label, property);
			Rect contentPosition = EditorGUI.PrefixLabel(position, label);

			SerializedProperty typeProp = property.FindPropertyRelative("type");
			PositionField.Type type = (PositionField.Type)typeProp.enumValueIndex;

			var width = contentPosition.width;

			EditorGUI.indentLevel = 0;
			{
				// 1. Modify the type
				contentPosition.width = width * typeWidth;
				EditorGUI.PropertyField(contentPosition, typeProp, GUIContent.none);
			}

			// 2. Modify the input depending on the type
			contentPosition.x += contentPosition.width + 4f;
			contentPosition.width = width * inputValueWidth;
			SerializedProperty inputProp = null;
			switch (type)
			{
				case PositionField.Type.Transform:
					inputProp = property.FindPropertyRelative(PositionField.transformFieldName);
					break;
				case PositionField.Type.Vector:
					inputProp = property.FindPropertyRelative(PositionField.pointFieldName);
					break;
			}

			EditorGUI.PropertyField(contentPosition, inputProp, GUIContent.none);
			EditorGUI.EndProperty();

			// 3. Save, if updated
			if (GUI.changed)
				property.serializedObject.ApplyModifiedProperties();
		}
	}

}