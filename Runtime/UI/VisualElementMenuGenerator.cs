using Stratus.Models.UI;

using System.Collections.Generic;
using System.Linq;

using UnityEngine.UIElements;

namespace Stratus.Unity.UI
{
	public class VisualElementMenuGenerator : MenuGenerator
	{
		public VisualElement root { get; }
		private List<Button> buttons = new List<Button>();

		public VisualElementMenuGenerator(VisualElement root)
		{
			this.root = root;
		}

		public override void Open(Menu menu)
		{
			Open(menu, true);
		}

		public override void Close()
		{
			Clear();
		}

		private void Open(Menu menu, bool focus)
		{
			Clear();
			foreach (var entry in menu)
			{
				if (entry is MenuItem item)
				{
					AddButton(item.name, item.action);
				}
				else if (entry is Menu subMenu)
				{
					Open(subMenu, true);
				}
			}
			if (focus)
			{
				buttons.First().Focus();
			}
		}

		private void AddButton(string name, MenuAction action)
		{
			var button = new Button();
			button.name = name;
			button.text = name;
			button.clicked += () => action();
			buttons.Add(button);
			root.Add(button);
		}

		private void Clear()
		{
			root.Clear();
			buttons.Clear();
		}
	}
}