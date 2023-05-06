using Stratus.Timers;

using UnityEngine;

namespace Stratus.Unity.Triggers
{
	/// <summary>
	/// After a specified amount of time, triggers the event
	/// </summary>
	public class StratusTimerTrigger : TriggerBehaviour
	{
		//------------------------------------------------------------------------/
		// Properties
		//------------------------------------------------------------------------/
		public override string automaticDescription => $"On {duration} seconds elapsed";

		//------------------------------------------------------------------------/
		// Fields
		//------------------------------------------------------------------------/
		[Header("Timer")]
		/// <summary>
		/// How long to wait before activating this trigger
		/// </summary>
		[Tooltip("How long to wait before activating this trigger")]
		public float duration;
		[Tooltip("Reset the current timer if the trigger is disabled")]
		public bool resetOnDisabled = true;
		private Countdown timer;

		//------------------------------------------------------------------------/
		// Messages
		//------------------------------------------------------------------------/
		protected override void OnAwake()
		{
			timer = new Countdown(duration);
			timer.WhenFinished(OnTimerFinished);
			timer.resetOnFinished = resetOnDisabled;
		}

		protected override void OnReset()
		{
		}

		private void Update()
		{
			timer.Update(Time.deltaTime);
		}

		private void OnEnable()
		{
			//this.RunTimer();
		}

		private void OnDisable()
		{
			if (resetOnDisabled)
				timer.Reset();
		}

		//------------------------------------------------------------------------/
		// Methods
		//------------------------------------------------------------------------/
		private void OnTimerFinished()
		{
			Activate();

		}

	}

}