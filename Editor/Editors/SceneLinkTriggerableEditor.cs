using Stratus.Editor;
using Stratus.Unity.Triggers;

using UnityEditor;

using UnityEngine;

namespace Stratus.Unity.Editor
{
	[CustomEditor(typeof(SceneLinkTriggerable))]
	public class SceneLinkTriggerableEditor : TriggerableEditor<SceneLinkTriggerable>
	{
		protected override void OnTriggerableEditorEnable()
		{
			propertyConstraints.Add(propertyMap["selectedScenes"], False);
			drawGroupRequests.Add(new DrawGroupRequest(SelectScenes, () => { return triggerable.scenePool != null; }));
		}

		private string GetName(StratusSceneField sceneField) => sceneField.name;

		private void SelectScenes2(Rect rect)
		{
			EditorGUILayout.BeginHorizontal();
			{
				EditorGUILayout.BeginVertical();
				{
					EditorGUILayout.LabelField("Available", EditorStyles.centeredGreyMiniLabel);
					foreach (var scene in triggerable.scenePool.scenes)
					{
						var matchingScene = triggerable.selectedScenes.Find(x => x.name == scene.name);
						if (matchingScene != null)
							continue;

						if (GUILayout.Button(scene.name, EditorStyles.miniButton))
						{
							triggerable.selectedScenes.Add(scene);
						}
					}
				}
				//EditorGUILayout.EndScrollView();
				EditorGUILayout.EndVertical();

				// Selected scenes
				EditorGUILayout.BeginVertical();
				{
					EditorGUILayout.LabelField("Selected", EditorStyles.centeredGreyMiniLabel);
					int indexToRemove = -1;
					for (int i = 0; i < triggerable.selectedScenes.Count; ++i)
					{
						var scene = triggerable.selectedScenes[i];
						if (GUILayout.Button(scene.name, EditorStyles.miniButton))
						{
							indexToRemove = i;
						}
					}
					if (indexToRemove > -1)
						triggerable.selectedScenes.RemoveAt(indexToRemove);
				}
				EditorGUILayout.EndVertical();
			}
			EditorGUILayout.EndHorizontal();
		}

		private void SelectScenes(Rect rect)
		{
			StratusEditorGUILayout.Subset(triggerable.scenePool.scenes, triggerable.selectedScenes, GetName);
		}
	}
}