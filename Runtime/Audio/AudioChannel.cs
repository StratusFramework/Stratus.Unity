using System;

namespace Stratus.Unity.Audio
{
	[Serializable]
	public class AudioChannel
	{
		public string name;
		public AudioPlayer player;

		public AudioChannel()
		{
		}

		public AudioChannel(string name)
		{
			this.name = name;
		}
	}

	/// <summary>
	/// The default audio channels used by Stratus components
	/// </summary>
	public enum StratusDefaultAudioChannel
	{
		/// <summary>
		/// Sound effects
		/// </summary>
		SFX,
		/// <summary>
		/// Voice-over
		/// </summary>
		VO,
		/// <summary>
		/// Music
		/// </summary>
		BGM
	}
}