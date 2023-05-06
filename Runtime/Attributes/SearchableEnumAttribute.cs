using System;

using UnityEngine;

namespace Stratus.Unity
{
	/// <summary>
	/// Displays an improved selected for enumerations that contains a search field
	/// </summary>
	[AttributeUsage(AttributeTargets.Field)]
	public class SearchableEnumAttribute : PropertyAttribute
	{
	}
}