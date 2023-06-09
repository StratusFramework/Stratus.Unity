using UnityEngine;

namespace Stratus.Unity.Events
{
	/// <summary>
	/// Whenever an Monobehaviour on a GameObject connects to the event system,
	/// this component is attached to it at runtime in order to handle book-keeping.
	/// It is only destroyed at the moment the GameObject is being destroyed.
	/// </summary>
	[DisallowMultipleComponent]
	[ExecuteInEditMode]
	public class EventsRegistration : MonoBehaviour
	{
		private void Awake()
		{
			this.hideFlags = HideFlags.HideAndDontSave | HideFlags.DontSaveInEditor;
			if (Application.isPlaying && !UnityEventSystem.IsConnected(this.gameObject))
			{
				UnityEventSystem.Connect(this.gameObject);
			}
		}

		void OnDestroy()
		{
			if (Application.isPlaying)
			{
				UnityEventSystem.Disconnect(this.gameObject);
			}
		}

	}
}