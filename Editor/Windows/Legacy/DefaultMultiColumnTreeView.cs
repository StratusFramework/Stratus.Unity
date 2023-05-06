using System.Collections.Generic;

using UnityEditor;
using UnityEditor.IMGUI.Controls;

using UnityEngine;

namespace Stratus.Unity.Editor
{
	public class DefaultMultiColumnTreeView : StratusMultiColumnTreeView<DefaultTreeElement, StratusDefaultColumn>
	{
		public DefaultMultiColumnTreeView(TreeViewState state, IEnumerable<DefaultTreeElement> data) : base(state, data)
		{
		}

		protected override TreeViewColumn BuildColumn(StratusDefaultColumn columnType)
		{
			TreeViewColumn column = null;
			switch (columnType)
			{
				case StratusDefaultColumn.Property:
					column = new TreeViewColumn
					{
						headerContent = new GUIContent(nameof(StratusDefaultColumn.Property)),
						minWidth = 200,
						width = 175,
						autoResize = true,
						sortedAscending = true,
						selectorFunction = (element) => element.element.name
					};
					break;
				case StratusDefaultColumn.Value:
					column = new TreeViewColumn
					{
						headerContent = new GUIContent(nameof(StratusDefaultColumn.Value)),
						sortedAscending = true,
						minWidth = 200,
						width = 400,
						autoResize = true,
						selectorFunction = (element) => element.element.value
					};
					break;
			}
			return column;
		}

		protected override void DrawColumn(Rect cellRect, StratusTreeViewItem<DefaultTreeElement> item, StratusDefaultColumn column, ref RowGUIArgs args)
		{
			switch (column)
			{
				case StratusDefaultColumn.Property:
					DefaultGUI.Label(cellRect, item.element.name, args.selected, args.focused);
					break;
				case StratusDefaultColumn.Value:
					DefaultGUI.Label(cellRect, item.element.value, args.selected, args.focused);
					break;
			}
		}

		protected override StratusDefaultColumn GetColumn(int index)
		{
			return (StratusDefaultColumn)index;
		}

		protected override int GetColumnIndex(StratusDefaultColumn columnType)
		{
			return (int)columnType;
		}

		protected override void OnContextMenu(GenericMenu menu)
		{

		}

		protected override void OnItemContextMenu(GenericMenu menu, DefaultTreeElement treeElement)
		{
			foreach (var action in treeElement.actions)
			{
				menu.AddItem(new GUIContent(action.label), false, () => action.action());
			}
		}

		protected override void OnItemDoubleClicked(DefaultTreeElement element)
		{
		}
	}

	public enum StratusDefaultColumn
	{
		Property,
		Value
	}
}