using UnityEngine;

using Event = Stratus.Events.Event;

namespace Stratus.Unity.Tilemaps
{
	/// <summary>
	/// The selection of a single tile within a tilemap
	/// </summary>
	public class CellSelection
	{
		public StratusTile tile { get; }
		public TileBehaviour behaviour { get; }
		public Vector2Int position { get; }
		public bool hasBehaviour => behaviour != null;

		public CellSelection(StratusTile tile, Vector2Int position)
		{
			this.tile = tile;
			this.position = position;
		}

		public CellSelection(StratusTile tile, Vector2Int position, TileBehaviour behaviour)
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

			CellSelection other = (CellSelection)obj;

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

	public abstract class CellSelectionEventBase : Event
	{
		public CellSelection selection { get; }

		protected CellSelectionEventBase(CellSelection selection)
		{
			this.selection = selection;
		}
	}

	public class CellSelectionEvent : CellSelectionEventBase
	{
		public CellSelectionEvent(CellSelection selection)
			: base(selection)
		{
		}
	}

	public class CellDeselectionEvent : CellSelectionEventBase
	{
		public CellDeselectionEvent(CellSelection selection)
			: base(selection)
		{
		}
	}
}