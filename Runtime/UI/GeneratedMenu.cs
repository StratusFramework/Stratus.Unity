using Stratus.Models.UI;

using UnityEngine;
using UnityEngine.UIElements;

namespace Stratus.Unity.UI
{
	public class GeneratedMenu : VisualElement
    {
		public new class UxmlFactory : UxmlFactory<GeneratedMenu> { }

		[SerializeField]
		private VisualTreeAsset uxml;
		
		private ScrollView items { get; set; }
		private VisualElementMenuGenerator generator { get; set; }

		public void Open(Menu menu)
		{
			if (items == null)
			{
				items = new ScrollView();
				items.name = "Items";
				Add(items);

				generator = new VisualElementMenuGenerator(items);
			}

			generator.Open(menu);
		}

		public void Close()
		{
			generator?.Close();
		}
	}
}
