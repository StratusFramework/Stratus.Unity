using UnityEngine;

namespace Stratus.Unity
{
	public enum HelpBoxMessageType
	{
		Info,
		Warning,
		Error
	}

	/// <summary>
	/// Shows a helpbox above a given field
	/// </summary>
	public class HelpBoxAttribute : PropertyAttribute
	{
		public string message;
		public HelpBoxMessageType messageType;

		public HelpBoxAttribute(string message, HelpBoxMessageType messageType = HelpBoxMessageType.Info)
		{
			this.message = message;
			this.messageType = messageType;
		}
	}
}