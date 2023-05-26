using UnityEngine;

using Event = Stratus.Events.Event;

namespace Stratus.Unity.Tilemaps
{
	/// <summary>
	/// The selection if a single tile within a tilemap
	/// </summary>
	public class TileSelection
	{
		public StratusTile tile { get; }
		public TileBehaviour behaviour { get; }
		public Vector2Int position { get; }
		public bool hasBehaviour => behaviour != null;

		public TileSelection(StratusTile tile, Vector2Int position)
		{
			this.tile = tile;
			this.position = position;
		}

		public TileSelection(StratusTile tile, Vector2Int position, TileBehaviour behaviour)
			: this(tile, position)
		{
			this.behaviour = behaviour;
		}

		public override string ToString()
		{
			return $"{tile} {position} <{(behaviour != null ? behaviour.name : string.Empty)}>";
		}

		public T As<T>() where T : StratusTile
		{
			return tile as T;
		}

		public override bool Equals(object obj)
		{
			if (obj == null)
			{
				return false;
			}

			TileSelection other = (TileSelection)obj;

			if (this.tile != other.tile)
			{
				return false;
			}

			if (this.behaviour != null
				&& other.behaviour != null)
			{
				return this.behaviour == other.behaviour;
			}
			else
			{
				return true;
			}
		}

		public override int GetHashCode()
		{
			return position.GetHashCode();
		}
	}

	public class TileSelectionEvent : Event
	{
		public TileSelection selection { get; }

		public TileSelectionEvent(TileSelection selection)
		{
			this.selection = selection;
		}
	}

	public class TileDeselectionEvent : Event
	{
	}
}