using UnityEngine;

namespace Stratus.Unity.Triggers
{
	public enum StratusMouseEventType
	{
		MouseDown,
		MouseUp,
		MouseEnter,
		MouseExit
	}

	[RequireComponent(typeof(Collider))]
	public class StratusMouseEventTrigger : TriggerBehaviour
	{
		public StratusMouseEventType eventType;

		protected override void OnAwake()
		{
		}

		protected override void OnReset()
		{
		}

		private void OnMouseDown()
		{
			if (eventType == StratusMouseEventType.MouseDown)
			{
				Activate();
			}
		}

		private void OnMouseUp()
		{
			if (eventType == StratusMouseEventType.MouseUp)
			{
				Activate();
			}
		}

		private void OnMouseEnter()
		{
			if (eventType == StratusMouseEventType.MouseEnter)
			{
				Activate();
			}
		}

		private void OnMouseExit()
		{
			if (eventType == StratusMouseEventType.MouseExit)
			{
				Activate();
			}
		}
	}

}