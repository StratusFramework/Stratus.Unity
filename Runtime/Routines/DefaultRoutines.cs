using Stratus.Unity.Utility;

using System.Collections;

using UnityEngine;

namespace Stratus.Unity.Routines
{
	/// <summary>
	/// A collection of useful coroutines
	/// </summary>
	public static class DefaultRoutines
	{
		public delegate void LerpFunction(float t);

		/// <summary>
		/// Invokes the given function every fixed update for a specified duration
		/// </summary>
		/// <param name="onupdate"></param>
		/// <param name="duration"></param>
		/// <param name="timeScale"></param>
		/// <returns></returns>
		public static IEnumerator OnFixedUpdate(System.Action onupdate, float duration)
		{
			while (duration > 0f)
			{
				onupdate();
				duration -= Time.fixedDeltaTime;
				yield return new WaitForFixedUpdate();
			}
		}

		/// <summary>
		/// A routine for waiting for a specific amount of time, in real time.
		/// This is mostly used when the time scale has been set to 0.
		/// </summary>
		/// <param name="time"></param>
		/// <returns></returns>
		public static IEnumerator WaitForRealSeconds(float time)
		{
			float start = Time.realtimeSinceStartup;
			while (Time.realtimeSinceStartup < start + time)
			{
				yield return null;
			}
		}



		/// <summary>
		/// A routine that given a duration, returns an increasing elapsed time (until it reaches the target duration)
		/// </summary>
		/// <param name="onUpdate"></param>
		/// <param name="duration"></param>
		/// <returns></returns>
		public static IEnumerator Elapse(System.Action<float> onUpdate, float duration, StratusTimeScale timeScale = StratusTimeScale.FixedDelta)
		{
			float elapsed = 0f;
			while (elapsed < duration)
			{
				elapsed += timeScale.GetTime();
				onUpdate(elapsed);
				yield return null;
			}
		}

		/// <summary>
		/// Invokes a function after specified amount of time
		/// </summary>
		/// <param name="onFinish"></param>
		/// <param name="delay"></param>
		/// <returns></returns>
		public static IEnumerator Call(System.Action onFinish, float delay)
		{
			yield return new WaitForSeconds(delay);
			onFinish();
		}

		/// <summary>
		/// A sequence of functions. First it will invoke a function before the loop begins, 
		/// a function while the loop runs for the specified duration, and a third function
		/// when the loop has run its course.
		/// </summary>
		/// <param name="onStart"></param>
		/// <param name="onUpdate"></param>
		/// <param name="onEnd"></param>
		/// <param name="duration"></param>
		/// <returns></returns>
		public static IEnumerator ThreeStep(System.Action onStart, System.Action onUpdate, System.Action onEnd, float duration)
		{
			onStart();

			float timeElapsed = 0f;
			while (timeElapsed <= duration)
			{
				timeElapsed += Time.deltaTime;
				onUpdate();
				yield return new WaitForFixedUpdate();
			}

			onEnd();
		}

		/// <summary>
		/// A sequence of functions. First it will invoke a function before the loop begins, 
		/// a function while the loop runs for the specified duration, and a third function
		/// when the loop has run its course.
		/// </summary>
		/// <param name="onStart"></param>
		/// <param name="onUpdate"></param>
		/// <param name="onEnd"></param>
		/// <param name="delay"></param>
		/// <returns></returns>
		public static IEnumerator InvokeWaitInvoke(System.Action onStart, float delay, System.Action onEnd)
		{
			onStart();

			float timeElapsed = 0f;
			while (timeElapsed <= delay)
			{
				timeElapsed += Time.deltaTime;
				yield return new WaitForFixedUpdate();
			}

			onEnd();
		}


		/// <summary>
		/// Runs the given coroutines in sequence until manually cancelled
		/// </summary>
		/// <param name="firstRoutine"></param>
		/// <param name="secondRoutine"></param>
		/// <returns></returns>
		public static IEnumerator Sequence(IEnumerator[] routines)
		{
			foreach (var routine in routines)
			{
				yield return routine;
			}
		}

		/// <summary>
		/// Toggles a boolean value from a given starting value to an ending one after a given period of time.
		/// </summary>
		/// <param name="boolean">A lambda expression of the form: '(x)=> boolean = x'</param>
		/// <param name="duration"></param>
		/// <returns></returns>
		public static IEnumerator Toggle(System.Action<bool> boolean, bool startingValue, bool endingValue, float duration)
		{
			boolean(startingValue);

			float timeElapsed = 0f;
			while (timeElapsed <= duration)
			{
				timeElapsed += Time.deltaTime;
				yield return new WaitForFixedUpdate();
			}

			boolean(endingValue);
		}

		public static IEnumerator Repeat(IEnumerator[] routines)
		{
			while (true)
			{
				yield return Sequence(routines);
			}
		}


	}

}
