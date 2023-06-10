using Stratus.Logging;
using Stratus.Unity.Extensions;

using System;

using UnityEngine;


namespace Stratus.Unity.Tilemaps
{
	/// <summary>
	/// Manages the loading and unloading of <see cref="MapBehaviour"/>
	/// </summary>
	public interface IMapManager
	{
		Camera camera { get; }
		MapBehaviour current { get; }
	}

	/// <summary>
	/// Manages the loading and unloading of <see cref="MapBehaviour"/>
	/// </summary>
	public class MapManager : StratusBehaviour, IMapManager
	{
		#region Fields
		[SerializeField]
		private Camera _camera;

		[Tooltip("If assigned, will be considered the initial map to load")]
		[SerializeReference]
		private MapBehaviour _map;

		[SerializeField]
		private Grid _grid;
		#endregion

		#region Properties
		/// <summary>
		/// The currently loaded map
		/// </summary>
		public MapBehaviour current { get; private set; }
		MapBehaviour IMapManager.current => current;
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
			if (_map != null)
			{
				Load(_map);
			}
			OnAwake();
		}

		private void Reset()
		{
			_camera = Camera.main;
			_grid = GetComponent<UnityEngine.Grid>();
			_map = GetComponentInChildren<MapBehaviour>();
		}
		#endregion

		#region Interface
		public void Load(MapBehaviour map, Action onFinished = null)
		{
			this.current = map;
			map.transform.SetParent(this.transform);
			map.Initialize(this);
			this.Log($"Loaded grid [{map}]");
			onFinished?.Invoke();
			onLoad?.Invoke();
		}

		public void Load()
		{
			if (_map == null)
			{
				throw new NullReferenceException("No map has been assigned");
			}
			Load(_map);
		}

		public void InstantiateAndLoad(MapBehaviour prefab)
		{
			var go = GameObject.Instantiate(prefab.gameObject, this.transform);
			var inst = go.GetComponent<MapBehaviour>();
			Load(inst);
		}

		public void Move(Transform transform, Vector2Int position)
		{
			Vector3 worldPosition = CellToWorld(position);
			transform.position = worldPosition;
		}

		public void Offset(Transform transform, Vector2Int offset)
		{
			var current = WorldToCell(transform.position);
			var target = current + offset;
			var position = CellToWorld(target);
			transform.position = position;
		}

		public Vector2Int WorldToCell(Vector3 world)
		{
			return _grid.WorldToCell(world).ToVector2Int();
		}

		/// <remarks>Uses the cell center</remarks>
		private Vector3 CellToWorld(Vector2Int cell)
		{
			return _grid.GetCellCenterWorld(cell.ToVector3Int());
		}



		public bool Unload()
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