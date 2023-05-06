using UnityEngine;

namespace Stratus.Unity.Triggers
{
	/// <summary>
	/// When triggered, itself triggers another <see cref="StratusTriggerableBehaviour"/>
	/// </summary>
	public class TriggerableProxy : TriggerableBehaviour
	{
		[Header("Targeting")]
		[Tooltip("What component to send the trigger event to")]
		public TriggerableBehaviour target;
		[Tooltip("Whether the trigger will be sent to the GameObject as an event or invoked directly on the dispatcher component")]
		public TriggerBehaviour.Scope delivery = TriggerBehaviour.Scope.GameObject;
		[Tooltip("Whether it should also trigger all of the object's children")]
		public bool recursive = false;

		protected override void OnAwake()
		{
		}

		protected override void OnReset()
		{

		}

		protected override void OnTrigger(object data = null)
		{
			if (this.delivery == TriggerBehaviour.Scope.GameObject)
			{
				if (!this.target)
				{
				}

				this.target.gameObject.Dispatch(new TriggerBehaviour.TriggerEvent());
				if (this.recursive)
				{
					foreach (var child in this.target.gameObject.Children())
					{
						child.Dispatch(new TriggerBehaviour.TriggerEvent());
					}
				}
			}

			else if (this.delivery == TriggerBehaviour.Scope.Component)
			{
				this.target.Trigger();
			}
		}
	}

}