using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

namespace Stratus.Unity.Editor
{
	public class ConsoleCommandEditorWindow : EditorWindow
	{
		[MenuItem("Window/UI Toolkit/ConsoleCommandEditorWindow")]
		public static void ShowExample()
		{
			ConsoleCommandEditorWindow wnd = GetWindow<ConsoleCommandEditorWindow>();
			wnd.titleContent = new GUIContent("ConsoleCommandEditorWindow");
		}

		public void CreateGUI()
		{
			// Each editor window contains a root VisualElement object
			VisualElement root = rootVisualElement;

			// VisualElements objects can contain other VisualElement following a tree hierarchy.
			VisualElement label = new Label("Hello World! From C#");
			root.Add(label);

			// Import UXML
			var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Stratus.Core/Editor/Windows/ConsoleCommandEditorWindow.uxml");
			VisualElement labelFromUXML = visualTree.Instantiate();
			root.Add(labelFromUXML);

			// A stylesheet can be added to a VisualElement.
			// The style will be applied to the VisualElement and all of its children.
			var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Stratus.Core/Editor/Windows/ConsoleCommandEditorWindow.uss");
			VisualElement labelWithStyle = new Label("Hello World! With Style");
			labelWithStyle.styleSheets.Add(styleSheet);
			root.Add(labelWithStyle);
		}
	}
}