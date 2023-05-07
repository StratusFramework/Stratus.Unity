using Stratus.Interpolation;
using Stratus.Unity.Data;

using System;

using UnityEngine;

namespace Stratus.Unity.Audio
{
	[Serializable]
	public class StratusAudioParameters
	{
		[Range(0f, 1f)]
		public float volume = 1f;
		[Range(0f, 1f)]
		public float pitch = 1f;
		public bool loop = false;

		public Ease fadeIn = Ease.Linear;
		public float fadeInDuration = 0f;
		public Ease fadeOut = Ease.Linear;
		public float fadeOutDuration = 0f;
	}

	[Serializable]
	public class AudioClipReference : UnityAssetReference<AudioClip>
	{
	}

	[CreateAssetMenu(menuName = scriptablesMenu + "Audio Clip Scriptable")]
	public class AudioClipCollectionScriptable : AssetCollectionScriptable<AudioClipReference>
	{
	}
}