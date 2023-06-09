using Stratus.Unity.Extensions;
using Stratus.Unity.Interpolation;

using System.Collections;

using UnityEngine;
using UnityEngine.UI;

namespace Stratus.Unity.Routines
{
	public static class RendererRoutines
	{
		public static IEnumerator Fade(Renderer[] renderers, float alpha, float duration, bool setActive)
		{
			foreach (Renderer renderer in renderers)
			{
				float diffAlpha = (alpha - renderer.material.color.a);

				float counter = 0f;
				while (counter < duration)
				{
					float alphaAmount = renderer.material.color.a + (Time.deltaTime * diffAlpha) / duration;
					renderer.material.color = new Color(renderer.material.color.r, renderer.material.color.g, renderer.material.color.b, alphaAmount);
					counter += Time.deltaTime;
					yield return null;
				}
				renderer.material.color = new Color(renderer.material.color.r, renderer.material.color.g, renderer.material.color.b, alpha);
				if (setActive)
				{
					renderer.transform.gameObject.SetActive(setActive);
				}
			}
		}

		public static IEnumerator Fade(this GameObject go, float alpha, float duration, bool setActive = true)
		{
			Renderer sr = go.GetComponent<Renderer>();
			float diffAlpha = (alpha - sr.material.color.a);

			float counter = 0;
			while (counter < duration)
			{
				float alphaAmount = sr.material.color.a + (Time.deltaTime * diffAlpha) / duration;
				sr.material.color = new Color(sr.material.color.r, sr.material.color.g, sr.material.color.b, alphaAmount);

				counter += Time.deltaTime;
				yield return null;
			}
			sr.material.color = new Color(sr.material.color.r, sr.material.color.g, sr.material.color.b, alpha);
			if (setActive)
			{
				sr.transform.gameObject.SetActive(setActive);
			}
		}

		public static IEnumerator Fade(Light light, Color color, float range, float intensity, float duration, StratusTimeScale timeScale = StratusTimeScale.Delta)
		{
			Color startColor = light.color;
			float startRange = light.range;
			float startIntensity = light.intensity;

			System.Action<float> func = (float t) =>
			{
				light.color = Color.Lerp(startColor, color, t);
				light.range = InterpolationRoutines.Lerp(startRange, range, t);
				light.intensity = InterpolationRoutines.Lerp(startIntensity, intensity, t);
			};

			yield return InterpolationRoutines.Lerp(func, duration, timeScale);
		}

		public static IEnumerator Blend(Light light, Color color, float intensity, float range, float duration, StratusTimeScale timeScale = StratusTimeScale.Delta)
		{
			Color startColor = light.color;
			float startRange = light.range;
			float startIntensity = light.intensity;

			System.Action<float> func = (float t) =>
			{
				light.color = Color.Lerp(startColor, color, t);
				light.range = InterpolationRoutines.Lerp(startRange, range, t);
				light.intensity = InterpolationRoutines.Lerp(startIntensity, intensity, t);
			};

			yield return InterpolationRoutines.Lerp(func, duration, timeScale);
		}

		public static IEnumerator CrossFadeAlpha(Image image, float alpha, float duration, StratusTimeScale timeScale = StratusTimeScale.Delta)
		{
			Color startColor = image.color;
			Color endColor = image.color.ScaleAlpha(alpha);

			System.Action<float> func = (float t) =>
			{
				image.color = Color.Lerp(startColor, endColor, t);
			};

			yield return InterpolationRoutines.Lerp(func, duration, timeScale);
		}
	}
}
