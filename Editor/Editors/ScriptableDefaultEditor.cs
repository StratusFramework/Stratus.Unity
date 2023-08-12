using Stratus.Unity.Data;

using UnityEditor;

namespace Stratus.Unity.Editor
{
	//[CanEditMultipleObjects]
	//[CustomEditor(typeof(StratusScriptable), true)]
	public class ScriptableDefaultEditor : ScriptableEditor<StratusScriptable>
	{
		protected override void OnStratusEditorEnable()
		{
		}
	}

}