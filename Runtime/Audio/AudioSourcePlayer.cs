﻿using Stratus.Extensions;
using Stratus.Interpolation;
using Stratus.Unity.Routines;

using System.Collections;

using UnityEngine;

namespace Stratus.Unity.Audio
{
	[RequireComponent(typeof(AudioSource))]
	public abstract class AudioSourcePlayer : AudioPlayer
	{
		public AudioSource audioSource => GetComponentCached<AudioSource>();
		public bool isPlaying => audioSource.isPlaying;
		public string currentAudioName { get; private set; }

		protected override void SetParameters(StratusAudioParameters parameters)
		{
			audioSource.volume = parameters.volume;
			audioSource.pitch = parameters.pitch;
			audioSource.loop = parameters.loop;
		}

		protected abstract Result OnPlay(string name);
		protected abstract Result OnStop();

		public override Result Play(string name)
		{
			currentAudioName = name;
			return OnPlay(name);
		}

		public override Result Stop()
		{
			if (audioSource.isPlaying)
			{
				return OnStop();
			}
			return false;
		}

		public override Result Pause(bool pause)
		{
			if (audioSource.isPlaying && pause)
			{
				if (pause)
				{
					audioSource.Pause();
				}
				return true;
			}
			else if (audioSource.clip != null && !pause)
			{
				audioSource.UnPause();
			}
			return false;
		}

		public override Result Mute(bool mute)
		{
			if (audioSource.isPlaying)
			{
				audioSource.mute = mute;
			}
			return false;
		}
	}

	public abstract class AudioSourcePlayer<AudioClipSource> : AudioSourcePlayer
		where AudioClipSource : IStratusAssetSource<AudioClipReference>, IStratusAssetResolver<AudioClipReference>
	{
		public AudioClipSource assets;
		private Coroutine playRoutine;
		private const string coroutineName = nameof(AudioSourcePlayer);

		protected override Result OnPlay(string name)
		{
			IEnumerator routine(AudioClipReference audioClip, StratusAudioParameters parameters)
			{
				float fadeOutDuration = isPlaying ? parameters.fadeOutDuration : 0f;
				bool fadeOutBlock = isPlaying;
				yield return FadeOutRoutine(fadeOutDuration, parameters.fadeOut, fadeOutBlock);

				audioSource.clip = audioClip.reference;
				audioSource.Play();

				yield return AudioRoutines.FadeVolume(audioSource, parameters.volume, parameters.fadeInDuration, parameters.fadeIn);
			}

			if (assets.HasAsset(name))
			{
				AudioClipReference audioClip = assets.GetAsset(name).asset;
				this.StartCoroutine(routine(audioClip, defaultParameters), coroutineName);
				return new Result(true, $"Now playing {name}");
			}

			return new Result(false, $"Could not find the audio asset {name.Enclose(StratusStringEnclosure.AngleBracket)}");
		}

		protected override Result OnStop()
		{
			IEnumerator routine()
			{
				yield return FadeOutRoutine(defaultParameters.fadeOutDuration, defaultParameters.fadeOut, isPlaying);
				audioSource.Stop();
				audioSource.clip = null;

			}
			this.StartCoroutine(routine(), coroutineName);
			return true;
		}

		IEnumerator FadeOutRoutine(float duration, Ease ease, bool block = true)
		{
			yield return AudioRoutines.FadeVolume(audioSource, 0f, duration, ease);
			if (block)
			{
				yield return new WaitForSeconds(duration);
			}
		}
	}
}