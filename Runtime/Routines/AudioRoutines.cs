using Stratus.Interpolation;
using Stratus.Unity.Interpolation;

using System.Collections;

using UnityEngine;

namespace Stratus.Unity.Routines
{
	public static class AudioRoutines
	{
		public static IEnumerator FadeVolume(AudioSource audioSource, float volume, float duration, Ease ease, StratusTimeScale timeScale = StratusTimeScale.Delta)
		{
			if (duration <= 0f)
			{
				audioSource.volume = volume;
				yield break;
			}

			float initial = audioSource.volume;
			void setVolume(float value) => audioSource.volume = value;
			yield return Interpolation.InterpolationRoutines.Interpolate(initial, volume, duration, setVolume, ease, null, timeScale);
		}

		public static IEnumerator FadePitch(AudioSource audioSource, float pitch, float duration, StratusTimeScale timeScale = StratusTimeScale.Delta)
		{
			if (duration <= 0f)
			{
				audioSource.pitch = pitch;
				yield break;
			}
			float initial = audioSource.pitch;
			System.Action<float> func = (float t) =>
			{
				audioSource.volume = InterpolationRoutines.Lerp(initial, pitch, t);
			};
			yield return InterpolationRoutines.Lerp(func, duration, timeScale);
		}
	}

}