using Stratus.Logging;

using System;

using UnityEngine;

namespace Stratus.Unity.Tilemaps
{
	/// <summary>
	/// Manages the loading and unloading of <see cref="GridBehaviour"/>
	/// </summary>
	public interface IGridManager
	{
		Camera camera { get; }
		GridBehaviour current { get; }
	}

	/// <summary>
	/// Manages the loading and unloading of <see cref="GridBehaviour"/>
	/// </summary>
	public class GridManager : StratusBehaviour, IGridManager
	{
		#region Fields
		[SerializeField]
		private Camera _camera;
		[SerializeField]
		[Tooltip("If assigned, will be considered the initial grid to load")]
		private GridBehaviour _grid;
		#endregion

		#region Properties
		public GridBehaviour current { get; private set; }		
		GridBehaviour IGridManager.current => current;
		public new Camera camera => _camera;
		#endregion

		#region Events
		public event Action onLoad;
		public event Action onUnload;
		#endregion

		#region Virtual
		protected virtual void OnAwake()
		{
		}
		#endregion

		#region Messages
		private void Awake()
		{
			if (_grid != null)
			{
				LoadGrid(_grid);
			}			
			OnAwake();
		}

		private void Reset()
		{
		}
		#endregion

		#region Interface
		public void LoadGrid(GridBehaviour grid, Action onFinished = null)
		{
			current = grid;
			grid.Initialize(this);
			this.Log($"Loaded grid [{grid}]");
			onFinished?.Invoke();
			onLoad?.Invoke();
		}

		public bool UnloadGrid()
		{
			if (current == null)
			{
				this.LogWarning("No map to unload");
				return false;
			}

			this.Log($"Unloading grid [{current}]");
			Destroy(current.gameObject);
			current = null;
			onUnload?.Invoke();
			return true;
		}
		#endregion
	}
}