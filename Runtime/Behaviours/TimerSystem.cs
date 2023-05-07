using Stratus.Timers;

using System.Collections.Generic;

using UnityEngine;

namespace Stratus.Unity.Behaviours
{
	/// <summary>
	/// A component which updates all timers registered to it, automatically.
	/// This makes it so you don't have to handle the updating yourself in your
	/// MonoBehaviour's update messages.
	/// </summary>
	public class TimerSystem : SingletonBehaviour<TimerSystem>
	{
		private List<Timer> All = new List<Timer>();

		protected override void OnAwake()
		{

		}

		private void Update()
		{
			foreach (var timer in All)
			{
				timer.Update(Time.deltaTime);
			}
		}

		public static void Add(Timer timer)
		{
			instance.All.Add(timer);
		}

		public static void Remove(Timer timer)
		{
			instance.All.Remove(timer);
		}
	}
}
