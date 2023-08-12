using Stratus.Logging;

using UnityEngine;
using UnityEngine.UIElements;

namespace Stratus.Unity.UI
{
	[RequireComponent(typeof(UIDocument))]
	public class DocumentBehaviour : StratusBehaviour
	{
		[SerializeField]
		private bool _visible;

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

		private void OnEnable()
		{
			if (!_visible)
			{
				visible = false;
			}
		}

		public virtual void Open()
		{
			visible = true;
			pickable = true;
			this.Log("Opening");
		}

		public virtual void Close()
		{
			this.Log("Closing");
			visible = false;
		}
	}
}
