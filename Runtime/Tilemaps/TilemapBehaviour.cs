using Stratus.Logging;
using Stratus.Unity.Extensions;

using System;

using UnityEngine;
using UnityEngine.Tilemaps;

namespace Stratus.Unity.Tilemaps
{
	/// <summary>
	/// A wrapper for a <see cref="Tilemap"/>, containing various utility functions
	/// </summary>
	public interface ITilemapBehaviour
    {
		string name { get; }
        void Initialize(Camera camera);
        CellSelection GetTile(Vector3Int position);
    }

    /// <summary>
    /// Instantiated in order to manage a specific tilemap
    /// </summary>
    /// <typeparam name="TileType"></typeparam>
    [Serializable]
    public class TilemapBehaviour : ITilemapBehaviour, IStratusLogger
    {
		#region Fields
		[SerializeField]
		private Tilemap _tilemap; 
		#endregion

		#region Properties
		public Tilemap tilemap => _tilemap;
		public Camera camera { get; private set; }
		public TilemapRenderer tilemapRenderer => tilemap.GetComponent<TilemapRenderer>();
		public Vector3Int mousePositionToCell => WorldToCell(camera.GetMousePositionToWorld());
		public BoundsInt bounds => tilemap.cellBounds;
		string ITilemapBehaviour.name => _tilemap.name;
		#endregion

		#region Messages
		public override string ToString()
		{
			return _tilemap.name;
		}
		#endregion

		#region Interface
		public void Initialize(Camera camera)
        {
            this.camera = camera;
        }

        public void Set(Tilemap tilemap)
        {
            _tilemap = tilemap;
        }

        public CellSelection GetTile<TileType>(Vector3Int position)
            where TileType : StratusTile
        {
            CellSelection selection = null;
            TileType tile = tilemap.GetTile<TileType>(position);
            if (tile != null)
            {
                selection = new CellSelection(tile, position.ToVector2Int());
            }
            return selection;
        }

        public CellSelection GetTile(Vector3Int position)
            => GetTile<StratusTile>(position);

        public void ToggleVisibility(bool toggle)
        {
            tilemapRenderer.enabled = toggle;
        }
		public CellSelection GetTileAtMousePosition()
		{
			Vector3 mousePos = camera.GetMousePositionToWorld();
			Vector3Int cellPos = WorldToCell(mousePos);
			return GetTile(cellPos);
		}

		public Vector3Int WorldToCell(Vector3 worldPosition)
		{
			return tilemap.WorldToCell(worldPosition);
		}

		public Vector3 CellCenterToWorld(Vector3Int cellPosition)
		{
			return tilemap.GetCellCenterWorld(cellPosition);
		}

		public bool HasTile(Vector3Int position) => tilemap.HasTile(position);

		public void MoveToTile(Transform transform, Vector3Int position)
		{
			CellSelection tile = GetTile(position);
			MoveToTile(transform, tile);
		}

		public void SetTile(Vector3Int position, StratusTile tile)
		{
			tilemap.SetTile(position, tile);
		}

		public void SetTile(Vector3Int position, StratusTile tile, Color color)
		{
			tilemap.SetTile(position, tile);
			SetTileColor(position, color);
		}

		public void RemoveTile(Vector3Int position)
		{
			tilemap.SetTile(position, null);
		}

		public void SetTileColor(Vector3Int position, Color color)
		{
			tilemap.SetTileFlags(position, TileFlags.None);
			tilemap.SetColor(position, color);
		}

		public void RemoveTileColor(Vector3Int position)
		{
			tilemap.RemoveTileFlags(position, TileFlags.None);
			tilemap.SetColor(position, Color.white);
		}

		public void MoveToTile(Transform transform, CellSelection tile)
		{
			var targetPos = tilemap.GetCellCenterWorld(tile.position.ToVector3Int());
			transform.position = targetPos;
		}

		public void ClearTiles()
		{
			tilemap.ClearAllTiles();
		}
		#endregion
	}
}