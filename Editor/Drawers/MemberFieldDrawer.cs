using UnityEditor;

namespace Stratus.Unity.Editor
{
	[CustomPropertyDrawer(typeof(StratusMemberField))]
	public class MemberFieldDrawer : SinglePropertyDrawer
	{
		protected override string childPropertyName => nameof(StratusMemberField.member);
	}

}