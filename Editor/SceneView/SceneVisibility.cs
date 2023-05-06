using UnityEditor;

namespace Stratus.Unity.Editor
{
	[InitializeOnLoad]
	public static class SceneVisibility
	{
		public static bool isolateCanvases => StratusPreferences.instance.isolateCanvases;
		public static bool isolated { get; private set; }
		public static bool enabled { get; private set; }

		static SceneVisibility()
		{
			Selection.selectionChanged += OnSelectionChanged;
			EditorApplication.playModeStateChanged += EditorApplication_playModeStateChanged;
		}

		private static void EditorApplication_playModeStateChanged(PlayModeStateChange obj)
		{
			switch (obj)
			{
				case PlayModeStateChange.ExitingPlayMode:
				case PlayModeStateChange.EnteredEditMode:
					enabled = true;
					break;

				case PlayModeStateChange.ExitingEditMode:
				case PlayModeStateChange.EnteredPlayMode:
					enabled = false;
					if (isolated)
					{
						CancelIsolation();
					}
					break;
			}
		}

		private static void OnSelectionChanged()
		{
			if (!enabled)
			{
				return;
			}

			var selectedGameObject = Selection.activeGameObject;
			if (selectedGameObject != null)
			{
				var isolationComponent = selectedGameObject.GetComponentInParent<IStratusSceneViewIsolate>();
				if (isolationComponent != null)
				{
					SceneVisibilityManager.instance.Isolate(isolationComponent.gameObject, true);
				}
				else
				{
					CancelIsolation();
				}

			}
			else if (isolated)
			{
				CancelIsolation();
			}
		}

		private static void CancelIsolation() => SceneVisibilityManager.instance.ExitIsolation();
	}

}