using System;

using UnityEngine;

namespace Stratus.Unity
{
	[AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
	public class DropdownAttribute : PropertyAttribute
	{
		public string memberName { get; private set; }

		public DropdownAttribute(string valuesName)
		{
			memberName = valuesName;
		}
	}

}