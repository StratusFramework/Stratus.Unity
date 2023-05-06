using Stratus.Editor;
using Stratus.IO;
using Stratus.Models;
using Stratus.Models.Graph;

using System.Collections.Generic;

using UnityEditor;
using UnityEditor.IMGUI.Controls;

using UnityEngine;

namespace Stratus.Unity.Editor
{
	public class SaveSystemWindow : StratusEditorWindow<SaveSystemWindow>
	{
		[SerializeField]
		private TreeViewState treeViewState = new TreeViewState();
		[SerializeField]
		private DefaultMultiColumnTreeView treeView;

		protected override void OnWindowEnable()
		{
			treeView = new DefaultMultiColumnTreeView(treeViewState, BuildTree());
		}

		protected override void OnWindowGUI()
		{
			this.treeView.TreeViewGUI(this.positionToGUI);
		}

		[MenuItem(StratusCore.rootMenu + "Save System")]
		private static void OpenFromMenu() => OpenWindow("Stratus Save System", true);

		private IEnumerable<DefaultTreeElement> BuildTree()
		{
			StratusSerializedTree<DefaultTreeElement> tree = new StratusSerializedTree<DefaultTreeElement>();
			//tree.AddElement(new StratusDefaultTreeElement(nameof(StratusSaveSystem.rootSaveDirectoryPath), StratusSaveSystem.rootSaveDirectoryPath,
			//	RevealPath(StratusSaveSystem.rootSaveDirectoryPath)), 0);
			return tree.elements;
		}

		/// <summary>
		/// Constructs an action that will reveal the given file/directory path
		/// </summary>
		public static LabeledAction RevealPath(string path)
		{
			return new LabeledAction("Reveal", () => StratusIO.Open(path));
		}
	}
}