using Stratus.Unity.Extensions;
using Stratus.Unity.Reflection;

using System.Collections.Generic;

using UnityEngine;

namespace Stratus.Unity.Behaviours
{
	public class GameObjectBookmark : EditorBehaviour<GameObjectBookmark>
	{
		//------------------------------------------------------------------------/
		// Fields
		//------------------------------------------------------------------------/
		/// <summary>
		/// Information about this GameObject
		/// </summary>    
		[SerializeField]
		private GameObjectInformation _information;

		//------------------------------------------------------------------------/
		// Properties
		//------------------------------------------------------------------------/
		public GameObjectInformation information => this._information;
		public static GameObjectInformation[] availableInformation { get; private set; } = new GameObjectInformation[0];
		public static bool hasAvailableInformation => availableInformation != null && availableInformation.Length > 0;
		public static System.Action onUpdate { get; set; } = new System.Action(() => { });

		//------------------------------------------------------------------------/
		// Messages
		//------------------------------------------------------------------------/
		protected override void OnReset()
		{
			this._information = new GameObjectInformation(this.gameObject);
		}

		protected override void OnStratusEditorBehaviourEnable()
		{
			if (this._information == null)
				this._information = new GameObjectInformation(gameObject);
			UpdateAvailable();
		}

		protected override void OnStratusEditorBehaviourDisable()
		{
			UpdateAvailable();
		}

		//------------------------------------------------------------------------/
		// Methods
		//------------------------------------------------------------------------/
		public static GameObjectBookmark Add(GameObject gameObject)
		{
			GameObjectBookmark bookmark = gameObject.AddComponent<GameObjectBookmark>();
			return bookmark;
		}

		public static void Remove(GameObject gameObject)
		{
			gameObject.RemoveComponent<GameObjectBookmark>();
		}

		public static void Toggle(GameObject gameObject)
		{
			if (gameObject.HasComponent<GameObjectBookmark>())
			{
				gameObject.RemoveComponent<GameObjectBookmark>();
			}
			else
			{
				gameObject.AddComponent<GameObjectBookmark>();
			}
		}

		public void SetInformation(GameObjectInformation information)
		{
			if (information.gameObject != this.gameObject)
				return;

			this._information = information.CloneJSON();
			this._information.UpdateMemberReferences();
			UpdateAvailable();
		}

		//------------------------------------------------------------------------/
		// Methods: Watch
		//------------------------------------------------------------------------/
		/// <summary>
		/// Updates the list of currently watched members
		/// </summary>
		public static void UpdateWatchList(bool invokeDelegate = false)
		{
			if (invokeDelegate)
			{
				onUpdate();
			}
		}

		/// <summary>
		/// Updates the list of available information from enabled bookmarks
		/// </summary>
		private static void UpdateInformation(bool invokeDelegate = false)
		{
			List<GameObjectInformation> availableInformation = new List<GameObjectInformation>();
			foreach (var bookmark in available)
			{
				availableInformation.Add(bookmark.Value.information);
			}
			GameObjectBookmark.availableInformation = availableInformation.ToArray();

			if (invokeDelegate)
			{
				onUpdate();
			}
		}

		/// <summary>
		/// Updates all available data for bookmarks
		/// </summary>
		public static void UpdateAvailable()
		{
			UpdateInformation();
			onUpdate();
		}

	}

}