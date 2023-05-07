using Stratus.Timers;
using Stratus.Utilities;

using System;
using System.Collections.Generic;

using UnityEngine;

namespace Stratus.Unity.Behaviours
{
	/// <summary>
	/// Manages timed updates for behaviours.
	/// </summary>
	[StratusSingleton(instantiate = true, isPlayerOnly = true, persistent = true, name = "Stratus Update System")]
	public class UpdateSystem : SingletonBehaviour<UpdateSystem>
	{
		public class FrequencyUpdateBatch
		{
			/// <summary>
			/// How often to update these methods
			/// </summary>
			private Countdown timer;
			/// <summary>
			/// The behaviours which to be updated
			/// </summary>
			private Dictionary<MonoBehaviour, List<Action>> behaviours;
			/// <summary>
			/// How many actions are being updated
			/// </summary>
			public int methodCount { get; private set; }
			/// <summary>
			/// How many actions are being updated
			/// </summary>
			public int behaviourCount => behaviours.Count;

			public FrequencyUpdateBatch(float frequency)
			{
				behaviours = new Dictionary<MonoBehaviour, List<Action>>();
				timer = new Countdown(frequency);
				timer.WhenFinished(Invoke);
				timer.resetOnFinished = true;
			}

			private void Invoke()
			{
				foreach (var behaviour in behaviours)
				{
					if (!behaviour.Key.enabled)
						continue;

					foreach (var action in behaviour.Value)
					{
						action.Invoke();
					}
				}
			}

			public void Add(Action action, MonoBehaviour behaviour)
			{
				if (!behaviours.ContainsKey(behaviour))
					behaviours.Add(behaviour, new List<Action>());

				behaviours[behaviour].Add(action);
				methodCount++;
			}

			public void Remove(MonoBehaviour behaviour)
			{
				if (!behaviours.ContainsKey(behaviour))
					return;

				methodCount -= behaviours[behaviour].Count;
				behaviours.Remove(behaviour);
			}

			public void Update(float time)
			{
				//Trace.Script($"time = {time}, progress = {timer.progress}");
				timer.Update(time);
			}

		}

		//------------------------------------------------------------------------/
		// Fields
		//------------------------------------------------------------------------/

		private Dictionary<float, FrequencyUpdateBatch> update = new Dictionary<float, FrequencyUpdateBatch>();

		private Dictionary<float, FrequencyUpdateBatch> fixedUpdate = new Dictionary<float, FrequencyUpdateBatch>();
		/// <summary>
		/// A list of timers being updated
		/// </summary>
		private List<Timer> timers = new List<Timer>();

		//------------------------------------------------------------------------/
		// Messages
		//------------------------------------------------------------------------/
		protected override void OnAwake()
		{
		}

		private void Update()
		{
			foreach (var batch in update)
			{
				batch.Value.Update(Time.deltaTime);
			}

			foreach (var timer in this.timers)
			{
				if (!timer.isFinished)
					timer.Update(Time.deltaTime);
			}
		}

		private void FixedUpdate()
		{
			foreach (var batch in update)
			{
				batch.Value.Update(Time.fixedDeltaTime);
			}
		}

		//------------------------------------------------------------------------/
		// Static Methods
		//------------------------------------------------------------------------/
		/// <summary>
		/// Subscribes a method to be invoked with the given frequency on the given timescale
		/// </summary>
		/// <param name="frequency"></param>
		/// <param name="action"></param>
		/// <param name="behaviour"></param>
		/// <param name="timeScale"></param>
		public static void Add(float frequency, Action action, MonoBehaviour behaviour, StratusTimeScale timeScale = StratusTimeScale.Delta)
		{
			instance.AddAction(frequency, action, behaviour, timeScale);
		}

		/// <summary>
		/// Adds a timer to be updated by the system
		/// </summary>
		/// <param name="frequency"></param>
		/// <param name="action"></param>
		/// <param name="behaviour"></param>
		/// <param name="timeScale"></param>
		public static void Add(Timer timer)
		{
			instance.timers.Add(timer);
		}

		/// <summary>
		/// Removes all subscribed methods for this behaviour on the given timescale
		/// </summary>
		/// <param name="frequency"></param>
		/// <param name="action"></param>
		/// <param name="behaviour"></param>
		/// <param name="timeScale"></param>
		public static void Remove(MonoBehaviour behaviour, StratusTimeScale timeScale = StratusTimeScale.Delta)
		{
			instance.RemoveBehaviour(behaviour, timeScale);
		}

		//------------------------------------------------------------------------/
		// Private Methods
		//------------------------------------------------------------------------/
		private void RemoveBehaviour(MonoBehaviour behaviour, StratusTimeScale timeScale = StratusTimeScale.Delta)
		{
			Dictionary<float, FrequencyUpdateBatch> selected = null;

			switch (timeScale)
			{
				case StratusTimeScale.Delta: selected = update; break;
				case StratusTimeScale.FixedDelta: selected = fixedUpdate; break;
			}

			foreach (var kp in selected)
			{
				kp.Value.Remove(behaviour);
			}
		}

		public void AddAction(float frequency, Action action, MonoBehaviour behaviour, StratusTimeScale timeScale = StratusTimeScale.Delta)
		{
			Dictionary<float, FrequencyUpdateBatch> selected = null;

			switch (timeScale)
			{
				case StratusTimeScale.Delta: selected = update; break;
				case StratusTimeScale.FixedDelta: selected = fixedUpdate; break;
			}

			if (!selected.ContainsKey(frequency))
				selected.Add(frequency, new FrequencyUpdateBatch(frequency));

			selected[frequency].Add(action, behaviour);
		}
	}
}
