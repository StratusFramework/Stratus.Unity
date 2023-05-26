using Stratus.Extensions;
using Stratus.Logging;
using Stratus.Models;
using Stratus.Models.Maps;
using Stratus.Search;
using Stratus.Unity.Events;
using Stratus.Unity.Extensions;
using Stratus.Utilities;

using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

namespace Stratus.Unity.Tilemaps
{
	/// <summary>
	/// Manages a <see cref="Grid"/>
	/// </summary>
	[RequireComponent(typeof(Grid))]
	public abstract class GridBehaviour : StratusBehaviour
	{
		#region Fields
		[SerializeField]
		private Grid _behaviour;


		#endregion
		public Map2D map { get; private set; }

		#region Properties
		/// <summary>
		/// The grid component
		/// </summary>
		public Grid behaviour => _behaviour;
		/// <summary>
		/// The base layer for the grid (usually its terrain)
		/// </summary>
		public abstract StratusTilemap baseLayer { get; }
		/// <summary>
		/// Each tilemap is a layer in this grid. These are by convention
		/// defined as fields then returned here.
		/// </summary>
		public abstract IStratusTilemap[] layers { get; }
		/// <summary>
		/// Camera used for the tilemap
		/// </summary>
		public Camera tilemapCamera { get; private set; }
		/// <summary>
		/// Spawned game objects
		/// </summary>
		private Dictionary<UnityEngine.Vector3Int, GameObject> spawnedGameObjects { get; set; }
			= new Dictionary<UnityEngine.Vector3Int, GameObject>();

		public Vector3 cellSize => behaviour.cellSize;
		#endregion

		#region Virtual
		/// <summary>
		/// Invoked when this behaviour is reset
		/// </summary>
		protected abstract void OnReset();
		/// <summary>
		/// Invoked internally to initialize this grid
		/// </summary>
		protected abstract void OnInitialize();

		protected virtual TraversableStatus IsBaseTileTraversible(StratusTile baseTile)
		{
			return TraversableStatus.Valid;
		}

		public virtual TraversableStatus IsTraversibleByBehaviour(UnityEngine.Vector3Int position, TileBehaviour behaviour)
		{
			return TraversableStatus.Valid;
		}
		#endregion

		#region Events
		public event Action<TileSelection> onSelectTile;
		#endregion

		#region Message
		private void Awake()
		{
			this.gameObject.Connect<TileBehaviourAddEvent>(OnBehaviourInstancedEvent);
			this.gameObject.Connect<TileBehaviourRemovedEvent>(OnBehaviourDestroyedEvent);
		}

		private void Reset()
		{
			_behaviour = GetComponent<Grid>();
			OnReset();
		}

		public override string ToString()
		{
			return name;
		}
		#endregion

		#region Behaviour Management
		private void OnBehaviourInstancedEvent(TileBehaviourAddEvent e)
		{
			InitializeBehaviourInstance(e.behaviour as TileBehaviour);
		}

		private void OnBehaviourDestroyedEvent(TileBehaviourRemovedEvent e)
		{
			RemoveBehaviourInstance(e.behaviour as TileBehaviour);
		}

		protected void InitializeBehaviourInstance(TileBehaviour behaviour)
		{
			if (behaviour == null)
			{
				return;
			}

			behaviour.Bind(this);
			if (!map.grid.Set(behaviour, behaviour.cellPosition))
			{
				this.LogError($"Failed to add behaviour {behaviour} to layer {behaviour.layer}");
				return;
			}

			// Set the move callback
			behaviour.onMoved += (sourcePosition, targetPosition) =>
			{
				UpdateBehaviourPosition(behaviour, targetPosition);
			};

			this.Log($"Added behaviour {behaviour} to layer {behaviour.layer}");
		}

		protected void RemoveBehaviourInstance(TileBehaviour behaviour)
		{
			if (behaviour == null)
			{
				return;
			}

			behaviour.Shutdown();
			if (map.grid.Remove(behaviour.layer, behaviour))
			{
				this.Log($"Removed behaviour {behaviour} to layer {behaviour.layer}");
			}
			else
			{
				this.Log($"Failed to remove behaviour {behaviour} to layer {behaviour.layer}");
			}
		}

		public void UpdateBehaviourPosition(TileBehaviour behaviour, Numerics.Vector2Int position)
		{
			map.grid.Set(behaviour, position);
			this.Log($"Updated {behaviour} position to {position}");
		}
		#endregion

		#region Setup
		/// <summary>
		/// To be invoked by the <see cref="IGridManager"/>
		/// </summary>
		/// <param name="camera"></param>
		public void Initialize(IGridManager manager)
		{
			this.tilemapCamera = manager.camera;
			baseLayer.Initialize(tilemapCamera);
			foreach (var layer in layers)
			{
				layer.Initialize(tilemapCamera);
			}
			OnInitialize();
			baseLayer.tilemap.CompressBounds();
			var grid = new Grid2D(layers.Select(l => l.name).Cast<Enumerated>(),
				new Bounds2D(baseLayer.bounds.size.ToNumericVector2Int()),
				CellLayout.Rectangle);
		}
		#endregion

		#region Selection
		/// <summary>
		/// Selects the behaviour or tile at the given world position (such as by a mouse)
		/// </summary>
		/// <param name="world"></param>
		public TileSelection SelectFromWorld(Vector3 world)
		{
			var local = WorldToLocal(world);
			var pos = LocalToCell(local);
			return Select(pos);
		}

		/// <summary>
		/// Selects the behaviour or tile at the cell position
		/// </summary>
		/// <param name="cellPos"></param>
		public TileSelection Select(Vector3Int cellPos)
		{
			// Get the tiel
			var tile = SelectTileAt(cellPos);

			// If there's a behaviour at the position...
			foreach(var layer in layers)
			{
				var target = map.grid.Get(layer.name, cellPos.ToNumericVector2Int());
				if (target != null)
				{
					return new TileSelection(tile.tile, tile.position);
				}
			}

			return tile;
		}

		/// <summary>
		/// Selects the tile at the given position
		/// </summary>
		/// <param name="cellPos"></param>
		public TileSelection SelectTileAt(Vector3Int cellPos)
		{
			foreach (var layer in layers.Reverse())
			{
				TileSelection selection = layer.GetTile(cellPos);
				if (selection != null)
				{
					return selection;
				}
			}
			return null;
		}
		#endregion

		#region Interface
		public bool ContainsCell(UnityEngine.Vector2Int position)
		{
			return baseLayer.bounds.Contains(position.ToVector3Int());
		}

		public Vector3 CellToWorld(UnityEngine.Vector3Int pos)
		{
			return behaviour.CellToWorld(pos);
		}

		public UnityEngine.Vector3Int WorldToCell(Vector3 world)
		{
			return behaviour.WorldToCell(world);
		}

		public Vector3 WorldToLocal(Vector3 world)
		{
			return behaviour.WorldToLocal(world);
		}

		public UnityEngine.Vector3Int LocalToCell(Vector3 world)
		{
			return behaviour.LocalToCell(world);
		}

		public Vector3 GetWorldPosition(UnityEngine.Vector3Int cellPosition)
		{
			return baseLayer.CellCenterToWorld(cellPosition);
		}
		#endregion

		#region Instancing
		public GameObject SpawnGameObject(UnityEngine.Vector3Int position, GameObject prefab)
		{
			if (spawnedGameObjects.ContainsKey(position))
			{
				this.LogError($"A gameobject has already been spawned at {position}");
				return null;
			}

			GameObject instance = GameObject.Instantiate(prefab, this.transform);
			instance.transform.position = GetWorldPosition(position);
			spawnedGameObjects.Add(position, instance);
			return instance;
		}
		#endregion
	}

	/// <summary>
	/// A grid with strongly-typed layers
	/// </summary>
	/// <typeparam name="TLayer"></typeparam>
	public abstract class GridBehaviour<TLayer> : GridBehaviour
		where TLayer : Enum
	{
		[Serializable]
		private class LayerTilemap : StratusTilemap
		{
			[ReadOnly]
			[SerializeField]
			private string _layer;

			public string layer => _layer;

			public LayerTilemap(string layer)
			{
				this._layer = layer;
			}

			public override string ToString()
			{
				return _layer;
			}
		}

		[SerializeField]
		private List<LayerTilemap> _layers;

		protected override void OnReset()
		{
			_layers = new List<LayerTilemap>();
			_layers.AddRange(EnumUtility.Names<TLayer>().Select(l => new LayerTilemap(l)));
		}

		public override IStratusTilemap[] layers => _layers.ToArray();
		public override StratusTilemap baseLayer => _layers.FirstOrDefault();

	}

}