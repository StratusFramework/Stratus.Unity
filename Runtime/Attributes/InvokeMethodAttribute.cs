using Stratus.Extensions;

using System;

namespace Stratus.Unity
{
	public class InspectorAttribute : Attribute
	{
		public string label { get; set; }
		public bool hasLabel => label.IsValid();

		public InspectorAttribute(string label)
		{
			this.label = label;
		}

		public InspectorAttribute()
		{
		}
	}

	/// <summary>
	/// A button that signals that this method can be invoked from an inspector
	/// </summary>
	[AttributeUsage(AttributeTargets.Method, Inherited = true, AllowMultiple = true)]
	public sealed class InvokeMethodAttribute : InspectorAttribute
	{
		/// <summary>
		/// Set this false to make the button not work whilst in playmode
		/// </summary>
		public bool isPlayMode { get; set; }

		public InvokeMethodAttribute(string label, bool isPlayMode = false)
			: base(label)
		{
			this.isPlayMode = isPlayMode;
		}

		public InvokeMethodAttribute()
		{
		}
	}
}
