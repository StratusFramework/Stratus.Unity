using Stratus.Unity.Rendering;

using System.Collections.Generic;

using UnityEditor;

namespace Stratus.Unity.Editor
{
	[InitializeOnLoad]
	public class SceneViewDisplayEditorWindow : EditorWindow
	{
		protected const string displayTitle = "Scene View Displays";
		private Dictionary<SceneViewDisplay, bool> expanded;
		private bool globalExpanded;

		static SceneViewDisplayEditorWindow()
		{
		}

		[MenuItem("Stratus/Core/Scene View Display")]
		public static void Open()
		{
			GetWindow(typeof(SceneViewDisplayEditorWindow), true, displayTitle);
		}

		private void OnEnable()
		{
			expanded = new Dictionary<SceneViewDisplay, bool>();
			foreach (var display in SceneViewDisplay.displays)
			{
				expanded[display] = false;
			}
		}

		private void OnGUI()
		{
			EditorGUILayout.BeginVertical(Styles.box);
			globalExpanded = EditorGUILayout.Foldout(globalExpanded, "Global");
			if (globalExpanded)
			{
				EditorGUI.indentLevel++;
				SceneViewDisplay.InspectGlobal();
				EditorGUI.indentLevel--;
			}
			EditorGUILayout.EndVertical();

			EditorGUILayout.BeginVertical(Styles.box);
			foreach (var display in SceneViewDisplay.displays)
			{
				expanded[display] = EditorGUILayout.Foldout(expanded[display], display.name);
				if (expanded[display])
				{
					EditorGUI.indentLevel++;
					EditorGUILayout.BeginVertical(Styles.box);
					display.Inspect();
					EditorGUILayout.EndVertical();
					EditorGUI.indentLevel--;
				}
			}
			EditorGUILayout.EndVertical();
		}
	}
}