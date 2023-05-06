using UnityEngine;

namespace Stratus.Unity
{
	/// <summary>
	/// Invokes a provided method when the value is changed
	/// </summary>
	public class ValueChangedAttribute : PropertyAttribute
	{
		public string method;

		public ValueChangedAttribute(string method)
		{
			this.method = method;
		}
	}
}