using Stratus.Logging;
using Stratus.Unity.Inputs;

using UnityEngine;
using UnityEngine.UIElements;

namespace Stratus.Unity.UI
{
	/// <summary>
	/// Base class for generated menus using a <see cref="UIDocument"/>
	/// </summary>
	/// <remarks>Requires the <see cref="UIDocument"/> to have one element in its tree that is <see cref="GeneratedMenu"/></remarks>
	[RequireComponent(typeof(UIDocument))]
	public abstract class MenuDocumentBehaviour : StratusBehaviour
	{
		private DefaultUserInterfaceInputLayer inputLayer = new DefaultUserInterfaceInputLayer();

		protected UIDocument document => GetComponentCached<UIDocument>();
		protected VisualElement root => document.rootVisualElement;
		private GeneratedMenu menu => root.Q<GeneratedMenu>();

		protected abstract Stratus.Models.UI.Menu GenerateMenu();

		private void OnEnable()
		{
			root.visible = false;
		}

		public void Open()
		{
			root.visible = true;
			this.Log("Opening");
			menu.Open(GenerateMenu());
			inputLayer.Push();
		}

		public void Close()
		{
			this.Log("Closing");
			inputLayer.Pop();
			menu?.Close();
			root.visible = false;
		}
	}
}
