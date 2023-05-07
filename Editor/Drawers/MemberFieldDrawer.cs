using Stratus.Unity.Reflection;

using UnityEditor;

namespace Stratus.Unity.Editor
{
	[CustomPropertyDrawer(typeof(MemberField))]
	public class MemberFieldDrawer : SinglePropertyDrawer
	{
		protected override string childPropertyName => nameof(MemberField.member);
	}

}