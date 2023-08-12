using Stratus.Logging;
using Stratus.Unity.Inputs;

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



		public override void Open()
		{
			base.Open();
			menu.Open(GenerateMenu());
			inputLayer.Push();
		}

		public override void Close()
		{
			inputLayer.Pop();
			menu?.Close();
			base.Close();
		}
	}
}
