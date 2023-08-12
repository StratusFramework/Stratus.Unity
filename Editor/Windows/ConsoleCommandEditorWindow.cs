using Stratus.Editor;
using Stratus.Extensions;
using Stratus.Systems;
using Stratus.Unity.Extensions;

using System.Linq;

using UnityEditor;
using UnityEditor.UIElements;

using UnityEngine;
using UnityEngine.UIElements;

namespace Stratus.Unity.Editor
{
	public class ConsoleCommandEditorWindow : StratusEditorWindow
	{
		private ToolbarSearchField searchField;
		private ListView view;

		private const string _title = "Console Command";

		[MenuItem(windowMenu + _title)]
		public static void OpenWindow()
		{
			OpenWindow<ConsoleCommandEditorWindow>(_title);
		}

		public void CreateGUI()
		{
			VisualElement root = rootVisualElement;

			// Import UXML
			var asset = Resources.Load<VisualTreeAsset>(nameof(ConsoleCommandEditorWindow));
			var tree = asset.Instantiate();
			root.Add(tree);

			// View
			view = tree.Q<ListView>();
			view.makeItem = MakeItem;
			view.bindItem = BindItem;
			view.itemsSource = ConsoleCommand.history.entries;
			Refresh();
			ConsoleCommand.onEntry += e => Refresh();

			// Toolbar
			var toolbar = tree.Q<Toolbar>();
			searchField = toolbar.Q<ToolbarSearchField>();
			searchField.RegisterCallback<KeyDownEvent>(e =>
			{
				if (e.keyCode == KeyCode.Return)
				{
					Submit();
					searchField.Focus();
				}
			});
			var menu = toolbar.Q<ToolbarMenu>();
			menu.text = $"{ConsoleCommand.commandsByName.Value.Count} Commands";
			foreach (var cmd in ConsoleCommand.commands.Value)
			{
				string name = cmd.name;
				menu.menu.AppendAction(name, action => searchField.value = name);
			}
			var submit = toolbar.Q<ToolbarButton>();
			submit.clicked += this.Submit;
			var clear = toolbar.Q<ToolbarButton>("Clear");
			clear.clicked += this.Clear;
		}

		private void BindItem(VisualElement visualElement, int index)
		{
			var entry = ConsoleCommand.entries[index];
			var label = visualElement as Label;

			Color color = default;
			string text = entry.text;

			switch (entry.type)
			{
				case ConsoleCommand.EntryType.Submit:
					color = Color.white;
					break;
				case ConsoleCommand.EntryType.Result:
					color = Color.green;
					break;
				case ConsoleCommand.EntryType.Warning:
					color = Color.yellow;
					break;
				case ConsoleCommand.EntryType.Error:
					color = Color.red;
					break;
			}

			label.text = $"[{entry.timestamp}] {text.ToRichText(color.ScaleSaturation(0.5f))}";
		}

		private VisualElement MakeItem()
		{
			var label = new Label();
			label.enableRichText = true;
			label.style.unityTextAlign = TextAnchor.MiddleLeft;
			return label;
		}

		private void Submit()
		{
			var text = searchField.value;
			ConsoleCommand.Submit(text);
			searchField.value = string.Empty;
		}

		private void Clear()
		{
			ConsoleCommand.ClearHistory();
			Refresh();
		}

		private void Refresh()
		{
			view.RefreshItems();
			view.ScrollToItem(view.childCount - 1);
		}
	}
}