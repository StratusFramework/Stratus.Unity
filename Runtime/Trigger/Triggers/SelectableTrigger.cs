using UnityEngine.UI;

namespace Stratus.Unity.Triggers
{
	public class SelectableTrigger : TriggerBehaviour
	{
		public Selectable selectable;
		public SelectableProxy.SelectionType type;
		public bool state;
		public SelectableProxy proxy { get; private set; }

		public override string automaticDescription
		{
			get
			{
				if (selectable)
					return $"On {selectable.gameObject.name}.{selectable.name} being {type} is {state}";
				return string.Empty;
			}
		}

		protected override void OnAwake()
		{
			proxy = SelectableProxy.Construct(selectable, type, OnSelection, persistent);
		}

		protected override void OnReset()
		{

		}

		void OnSelection(bool state)
		{
			if (this.state != state)
				return;

			Activate();
		}



	}
}
