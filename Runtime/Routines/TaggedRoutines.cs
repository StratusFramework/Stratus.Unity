﻿using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace Stratus.Unity.Routines
{
	public static class TaggedRoutines
	{
		private class RoutineActivity
		{
			private static int created;
			public int id;
			public MonoBehaviour monoBehaviour;
			public Coroutine coroutine;
			public bool isActive;

			public RoutineActivity(MonoBehaviour monoBehaviour)
			{
				this.id = created++;
				this.monoBehaviour = monoBehaviour;
			}

			public void Stop()
			{
				isActive = false;
				monoBehaviour.StopCoroutine(coroutine);
			}
		}

		private static Dictionary<MonoBehaviour, Dictionary<string, RoutineActivity>> monobehaviourRoutines = new Dictionary<MonoBehaviour, Dictionary<string, RoutineActivity>>();

		public static void StartCoroutine(MonoBehaviour mb, IEnumerator routine, string tag, System.Action onFinished = null)
		{
			RoutineActivity current = null;

			// Check if this monobehaviour has already been used before
			if (!monobehaviourRoutines.ContainsKey(mb))
			{
				monobehaviourRoutines.Add(mb, new Dictionary<string, RoutineActivity>());
			}

			// Stop an already running coroutine if present
			bool isPresent = monobehaviourRoutines[mb].TryGetValue(tag, out current);
			if (isPresent)
			{
				current.Stop();
			}
			// Otherwise add it to the list
			else
			{
				current = new RoutineActivity(mb);
				monobehaviourRoutines[mb].Add(tag, current);
			}

			// Now add/replace the current routine
			if (onFinished != null)
			{
				IEnumerator composedRoutine = ComposeRoutineWithCallback(mb, current, routine, onFinished);
				if (mb.isActiveAndEnabled)
					current.coroutine = mb.StartCoroutine(composedRoutine);
			}
			else
			{
				if (mb.isActiveAndEnabled)
					current.coroutine = mb.StartCoroutine(routine);
			}

			// Start it
			//Trace.Script("Starting " + tag);

		}

		public static void StopCoroutine(MonoBehaviour mb, string tag)
		{
			if (!monobehaviourRoutines.ContainsKey(mb))
				return;

			RoutineActivity current = null;

			// Stop an already running coroutine if present
			bool isPresent = monobehaviourRoutines[mb].TryGetValue(tag, out current);
			if (isPresent)
			{
				//Trace.Script("Stopping " + tag);
				current.Stop();
			}
		}

		private static void StopCoroutine(RoutineActivity routineActivity)
		{
			if (routineActivity.coroutine != null)
				routineActivity.monoBehaviour.StopCoroutine(routineActivity.coroutine);
			routineActivity.isActive = false;
		}

		private static IEnumerator ComposeRoutineWithCallback(MonoBehaviour mb, RoutineActivity activity, IEnumerator routine, System.Action onFinished)
		{
			activity.isActive = true;
			while (activity.isActive && routine.MoveNext())
			{
				yield return routine.Current;
			}
			//yield return routine;
			activity.isActive = false;
			onFinished?.Invoke();
		}

	}
}
