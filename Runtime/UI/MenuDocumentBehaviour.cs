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
	public abstract class MenuDocumentBehaviour : DocumentBehaviour
	{
		private DefaultUserInterfaceInputLayer inputLayer = new DefaultUserInterfaceInputLayer();				
		private GeneratedMenu menu => root.Q<GeneratedMenu>();

		protected abstract Stratus.Models.UI.Menu GenerateMenu();

		private void OnEnable()
		{
			visible = false;
		}

		public void Open()
		{
			visible = true;
			pickable = true;
			this.Log("Opening");
			menu.Open(GenerateMenu());
			inputLayer.Push();
		}

		public void Close()
		{
			this.Log("Closing");
			inputLayer.Pop();
			menu?.Close();
			visible = false;
		}
	}

	[RequireComponent(typeof(UIDocument))]
	public abstract class DocumentBehaviour : StratusBehaviour
	{
		protected UIDocument document => GetComponentCached<UIDocument>();
		protected VisualElement root => document.rootVisualElement;

		public bool visible
		{
			get => root.visible;
			set => root.visible = value;
		}

		protected bool pickable
		{
			set => root.pickingMode = value ?
				PickingMode.Position :
				PickingMode.Ignore;
			get => root.pickingMode == PickingMode.Position;
		}

		protected void EnablePicking() => pickable = true;
	}
}
