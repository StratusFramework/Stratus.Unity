using System;

using UnityEngine;

namespace Stratus.Unity.Data
{
	public enum OptionValue
	{
		Default,
		True,
		False
	}

	[Serializable]
	public class OverrideField<T> where T : struct
	{
		/// <summary>
		/// Whether to use this override
		/// </summary>
		[Tooltip("Whether to use this override")]
		public bool enabled;

		/// <summary>
		/// The value to use as an override
		/// </summary>
		[Tooltip("The value to use as an override")]
		public T value;

		/// <summary>
		/// Based on whether this field is enabled, retrieves its value
		/// (or uses the default provided)
		/// </summary>
		/// <param name="defaultValue"></param>
		/// <returns></returns>
		public T Get(ref T defaultValue) => enabled ? value : defaultValue;
	}

	public class StratusFloatOverride : OverrideField<float> { }
	public class StratusIntOverride : OverrideField<float> { }
	public class StratusVector3Override : OverrideField<float> { }
}