using Stratus;
using Stratus.Collections;
using Stratus.Logging;
using Stratus.Utilities;

using System;
using System.Collections.Generic;

using UnityEngine;

namespace Stratus.Unity.Audio
{
	[StratusSingleton(instantiate = false)]
	public class AudioSourceChannelPlayer : StratusSingletonBehaviour<AudioSourceChannelPlayer>
	{
		[SerializeField]
		private bool debug = false;
		[SerializeField]
		private List<AudioChannel> _channels = new List<AudioChannel>();
		public AutoSortedList<string, AudioChannel> channelsByName { get; private set; }

		protected override void OnAwake()
		{
			RegisterChannels();
		}

		private void Reset()
		{
			_channels = new List<AudioChannel>();
			foreach (var channel in EnumUtility.Values<StratusDefaultAudioChannel>())
			{
				_channels.Add(new AudioChannel(channel.ToString()));
			}
		}

		private void RegisterChannels()
		{
			channelsByName = new AutoSortedList<string, AudioChannel>(x => x.name, _channels.Count, StringComparer.InvariantCultureIgnoreCase);
			channelsByName.AddRange(_channels);
		}

		public Result Play(string channel, string name)
		{
			return InvokeAudioChannel(channel, (player) => player.Play(name));
		}

		public Result Stop(string channel)
		{
			return InvokeAudioChannel(channel, (player) => player.Stop());
		}

		/// <summary>
		/// Stops playback on all channels
		/// </summary>
		public void Stop()
		{
			foreach (var channel in _channels)
			{
				channel.player.Stop();
			}
		}

		public Result Pause(string channel, bool pause)
		{
			return InvokeAudioChannel(channel, (player) => player.Pause(pause));
		}

		public Result Pause(string channel) => InvokeAudioChannel(channel, (player) => player.Pause());
		public Result Resume(string channel) => InvokeAudioChannel(channel, (player) => player.Resume());

		public Result Mute(string channel, bool mute)
		{
			return InvokeAudioChannel(channel, (player) => player.Mute(mute));
		}

		private Result InvokeAudioChannel(string channel, Func<AudioPlayer, Result> onChannel)
		{
			Result result;
			if (!channelsByName.ContainsKey(channel))
			{
				result = new Result(false, $"The audio channel {channel} was not found!");
			}
			else
			{
				result = onChannel(channelsByName[channel].player);
			}
			this.Log(result);
			return result;
		}
	}
}