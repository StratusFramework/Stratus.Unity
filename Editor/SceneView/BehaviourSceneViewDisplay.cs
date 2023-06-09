using Stratus.Unity.Scenes;

using UnityEditor;

using UnityEngine;

namespace Stratus.Unity.Editor
{
	/// <summary>
	/// Supports listening to the hierarchy window's hierarchyWindowItemOnGUI delegate
	/// </summary>
	public interface IHierarchyWindowItemOnGUI
	{
		void OnHierarchyWindowItemOnGUI(int instanceID, Rect selectionRect);
	}

	public interface IUnityGUIEventListener
	{
		Event currentEvent { get; set; }
		Vector2 mousePosition { get; set; }
	}

	/// <summary>
	/// An abstract interface for writing a SceneView display for a MonoBehaviour of a given type
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public abstract class BehaviourSceneViewDisplay<T> : SceneViewDisplay where T : MonoBehaviour
	{
		private T[] behaviours;
		protected abstract void OnInspect(T behaviour);

		protected Vector2 mousePosition;
		protected Event currentEvent;

		protected override void OnInitializeState()
		{
			behaviours = StratusScene.GetComponentsInAllActiveScenes<T>();
		}

		protected override void OnInitializeDisplay()
		{
			var hierarchyWindowOnGUIListener = this as IHierarchyWindowItemOnGUI;
			if (hierarchyWindowOnGUIListener != null)
				EditorApplication.hierarchyWindowItemOnGUI += hierarchyWindowOnGUIListener.OnHierarchyWindowItemOnGUI;

			OnInitializeState();
		}

		protected override void OnSceneGUI(SceneView view)
		{
			foreach (var behaviour in behaviours)
				OnInspect(behaviour);
		}

		protected override void OnReset()
		{
		}

		protected override void OnInspect()
		{

		}

	}


}