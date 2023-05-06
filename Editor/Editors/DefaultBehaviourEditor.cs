using UnityEditor;

namespace Stratus.Unity.Editor
{
	[CanEditMultipleObjects]
	[CustomEditor(typeof(StratusBehaviour), true)]
	public class DefaultBehaviourEditor : BehaviourEditor<StratusBehaviour>
	{
		protected override void OnStratusEditorEnable()
		{
		}
	}

}