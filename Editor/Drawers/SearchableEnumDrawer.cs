using UnityEditor;

using UnityEngine;

namespace Stratus.Unity.Editor
{
	[CustomPropertyDrawer(typeof(SearchableEnumAttribute))]
	public class SearchableEnumDrawer : PropertyDrawer
	{
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			label = EditorGUI.BeginProperty(position, label, property);
			{
				SearchableEnum.EnumPopup(position, label, property);
			}
			EditorGUI.EndProperty();
		}
	}
}