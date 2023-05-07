using Stratus.Unity.Data;

using UnityEngine;

namespace Stratus.Unity.Scenes
{
	[CreateAssetMenu(fileName = "Scene Pool", menuName = "Stratus/Scene Pool")]
	public class ScenePool : StratusScriptable
	{
		/// <summary>
		/// The list of scenes being used in the project
		/// </summary>
		public SceneField[] scenes;

		/// <summary>
		/// The initial scene to be loaded upon entering play mode (will be the first one listed on the scenes array)
		/// </summary>
		public SceneField initialScene
		{
			get
			{
				if (scenes.Length > 0)
					return scenes[0];
				return null;
			}
		}

		/// <summary>
		/// Opens all scenes in the editor
		/// </summary>
		public void OpenAll(StratusScene.SceneCallback onOpened = null)
		{
			StratusScene.Load(scenes, onOpened);
		}

		/// <summary>
		/// Closes all scenes in the editor
		/// </summary>
		public void CloseAll(StratusScene.SceneCallback onClosed = null)
		{
			StratusScene.Unload(scenes, onClosed);
		}
	}
}
