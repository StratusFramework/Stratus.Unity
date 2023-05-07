using Stratus.Interpolation;
using Stratus.Unity.Routines;
using Stratus.Unity.Utility;

using System.Collections;

using UnityEngine;

namespace Stratus.Unity.Interpolation
{
	public static class InterpolationRoutines
	{
		/// <summary>
		/// A routine for linearly interpolating between two values 
		/// a and b by the interpolant t. This parameter is clamped to the range [0,1]
		/// </summary>
		/// <param name="onUpdate">The function to call each update with the t value passed to it.</param>
		/// <param name="duration">The duration of this interpolation.</param>
		/// <returns></returns>
		public static IEnumerator Lerp(System.Action<float> onUpdate, float duration, StratusTimeScale timeScale = StratusTimeScale.FixedDelta)
		{
			if (duration == 0f)
			{
				onUpdate(1f);
			}
			else
			{
				float t = 0f;
				while (t <= 1f)
				{
					t += timeScale.GetTime() / duration;
					onUpdate(t);
					yield return timeScale.Yield();
				}
			}
		}

		/// <summary>
		/// Commonly used for alpha blending
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <param name="t"></param>
		/// <returns></returns>
		public static float Lerp(float a, float b, float t)
		{
			return (1 - t) * a + t * b;
		}

		public static IEnumerator Lerp<T>(T initialValue, T finalValue, float duration, System.Action<T> setter, System.Func<T, T, float, T> lerpFunction, System.Action onFinished = null, StratusTimeScale timeScale = StratusTimeScale.Delta)
		{
			System.Action<float> lerp = (t) =>
			{
				T currentValue = lerpFunction(initialValue, finalValue, t);
				setter.Invoke(currentValue);
			};

			yield return Lerp(lerp, duration, timeScale);
			setter.Invoke(finalValue);
			onFinished?.Invoke();
		}

		public static IEnumerator Interpolate(float initialValue, float finalValue, float duration, System.Action<float> setter, Ease ease = Ease.Linear, System.Action onFinished = null, StratusTimeScale timeScale = StratusTimeScale.Delta)
		{
			float diff = finalValue - initialValue;
			EaseUtility.EaseFunction easeFunc = ease.ToFunction();

			System.Action<float> lerp = (t) =>
			{
				float currentValue = initialValue + diff * easeFunc(t);
				setter.Invoke(currentValue);
			};

			yield return Lerp(lerp, duration, timeScale);
			setter.Invoke(finalValue);
			onFinished?.Invoke();
		}

		public static IEnumerator Interpolate(int initialValue, int finalValue, float duration, System.Action<int> setter, Ease ease = Ease.Linear, System.Action onFinished = null, StratusTimeScale timeScale = StratusTimeScale.Delta)
		{
			float diff = finalValue - initialValue;
			EaseUtility.EaseFunction easeFunc = ease.ToFunction();

			System.Action<float> lerp = (t) =>
			{
				float currentValue = initialValue + diff * easeFunc(t);
				setter.Invoke(Mathf.CeilToInt(currentValue));
			};

			yield return Lerp(lerp, duration, timeScale);
			setter.Invoke(finalValue);
			onFinished?.Invoke();
		}

		public static IEnumerator Interpolate(bool initialValue, bool finalValue, float duration, System.Action<bool> setter, System.Action onFinished = null, StratusTimeScale timeScale = StratusTimeScale.Delta)
		{
			yield return new WaitForSeconds(duration);
			setter.Invoke(finalValue);
			onFinished?.Invoke();
		}

		public static IEnumerator Interpolate(Vector2 initialValue, Vector2 finalValue, float duration, System.Action<Vector2> setter, Ease ease = Ease.Linear, System.Action onFinished = null, StratusTimeScale timeScale = StratusTimeScale.Delta)
		{
			Vector2 diff = finalValue - initialValue;
			EaseUtility.EaseFunction easeFunc = ease.ToFunction();

			System.Action<float> lerp = (t) =>
			{
				Vector2 currentValue = initialValue + diff * easeFunc(t);
				setter.Invoke(currentValue);
			};

			yield return Lerp(lerp, duration, timeScale);
			setter.Invoke(finalValue);
			onFinished?.Invoke();
		}

		public static IEnumerator Interpolate(Vector3 initialValue, Vector3 finalValue, float duration, System.Action<Vector3> setter, Ease ease = Ease.Linear, System.Action onFinished = null, StratusTimeScale timeScale = StratusTimeScale.Delta)
		{
			Vector3 diff = finalValue - initialValue;
			EaseUtility.EaseFunction easeFunc = ease.ToFunction();

			System.Action<float> lerp = (t) =>
			{
				Vector3 currentValue = initialValue + diff * easeFunc(t);
				setter.Invoke(currentValue);
			};

			yield return Lerp(lerp, duration, timeScale);
			setter.Invoke(finalValue);
			onFinished?.Invoke();
		}

		public static IEnumerator Interpolate(Vector4 initialValue, Vector4 finalValue, float duration, System.Action<Vector4> setter, Ease ease = Ease.Linear, System.Action onFinished = null, StratusTimeScale timeScale = StratusTimeScale.Delta)
		{
			Vector4 diff = finalValue - initialValue;
			EaseUtility.EaseFunction easeFunc = ease.ToFunction();

			System.Action<float> lerp = (t) =>
			{
				Vector4 currentValue = initialValue + diff * easeFunc(t);
				setter.Invoke(currentValue);
			};

			yield return Lerp(lerp, duration, timeScale);
			setter.Invoke(finalValue);
			onFinished?.Invoke();
		}

		public static IEnumerator Interpolate(Color initialValue, Color finalValue, float duration, System.Action<Color> setter, Ease ease = Ease.Linear, System.Action onFinished = null, StratusTimeScale timeScale = StratusTimeScale.Delta)
		{
			Color diff = finalValue - initialValue;
			EaseUtility.EaseFunction easeFunc = ease.ToFunction();

			System.Action<float> lerp = (t) =>
			{
				Color currentValue = initialValue + diff * easeFunc(t);
				setter.Invoke(currentValue);
			};

			yield return Lerp(lerp, duration, timeScale);
			setter.Invoke(finalValue);
			onFinished?.Invoke();
		}
	}
}