using UnityEngine;

namespace Stratus.Unity
{
	/// <summary>
	/// Allows tags to be set using a drop-down popup
	/// </summary>
	/// <remarks>Reference: http://www.brechtos.com/tagselectorattribute/</remarks>
	public class TagAttribute : PropertyAttribute
	{
		public bool UseDefaultTagFieldDrawer = false;
	}

}