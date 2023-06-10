using Stratus.Logging;
using Stratus.Models.Maps;
using Stratus.Unity.Events;
using Stratus.Unity.Extensions;

using System;

using UnityEngine;

using Event = Stratus.Events.Event;

namespace Stratus.Unity.Tilemaps
{


	public class GridNavigationRange
	{
		public Vector2Int firstSelection;
		public void Highlight()
		{
		}
		public Vector2Int GetNextCellPosition(Vector3 direction)
		{
			throw new NotImplementedException();
		}
		public void OnNavigateToCellPosition(Vector2 position) => throw new NotImplementedException();
		public bool Contains(Vector2Int position) => throw new NotImplementedException();
	}

	public interface IGridNavigation
	{
		MapBehaviour grid { get; }
		void NavigateToCellPosition(Vector2Int position);
		Vector2Int currentCellPosition { get; }
	}

	public class GridNavigator : StratusBehaviour, IGridNavigation
	{
		[SerializeField]
		private MapInput input;
		[SerializeField]
		private SpriteRenderer cursor;

		public MapBehaviour grid => input.map.current;
		public Vector2Int currentCellPosition { get; private set; }
		public Vector2Int? previousCellPosition { get; private set; }
		public GridNavigationRange navigationRange
		{
			get => _navigationRange;
			set
			{
				_navigationRange = value;
				if (_navigationRange != null)
				{
					NavigateToCellPosition(_navigationRange.firstSelection);
					_navigationRange.Highlight();
					this.Log($"Navigation range set to {value}");
				}
			}
		}
		private GridNavigationRange _navigationRange;

		private void Awake()
		{
			this.gameObject.Connect<TileSelectionEvent>(e =>
			{
				SetCellPosition(e.selection.position);
			});
			this.gameObject.Connect((Action<TileDeselectionEvent>)(e =>
			{
				HideCursor();
			}));
			this.gameObject.Connect<NavigateGridEvent>(e =>
			{
				NavigateCellDirection(e.direction.ToUnityVector2Int());
			});
		}

		private void HideCursor()
		{
			cursor.gameObject.SetActive(false);
		}

		private void Start()
		{
			ScaleToGrid();
		}

		private void Reset()
		{
			input = GetComponent<MapInput>();
		}

		private void OnStratusTileSelectionEvent(TileSelectionEvent e)
		{

		}

		[InvokeMethod]
		public void ScaleToGrid()
		{
			// TODO: Implement a way to scale the cursor for the grid
			//var ppu = e.selection.tile.sprite.pixelsPerUnit;
			//cursor.transform.localScale = size;
		}

		public void NavigateCellDirection(Vector2 direction)
		{
			if (navigationRange != null)
			{
				NavigateToCellPosition(navigationRange.GetNextCellPosition(direction));
				return;
			}

			int x = (int)direction.x;
			int y = (int)direction.y;

			Vector2Int nextCellPosition = currentCellPosition;
			nextCellPosition.x += x;
			nextCellPosition.y += y;

			// For vertical movement...
			if (y != 0)
			{
				// Flip the horizontal direction and try again
				if (!CanNavigateToCell(nextCellPosition))
				{
					nextCellPosition.x += (currentCellPosition.x > 0 ? -1 : 1);
				}
			}

			NavigateToCellPosition(nextCellPosition);
		}

		public void NavigateToCellPosition(Vector2Int position)
		{
			if (CanNavigateToCell(position))
			{
				SetCellPosition(position);
			}
		}

		private void SetCellPosition(Vector2Int position)
		{
			this.Log($"Navigating cell at {position}");
			previousCellPosition = currentCellPosition;
			currentCellPosition = position;

			var worldPos = grid.GetWorldPosition(position.ToVector3Int());
			cursor.transform.position = worldPos;
			cursor.gameObject.SetActive(true);

			if (navigationRange != null)
			{
				navigationRange.OnNavigateToCellPosition(currentCellPosition);
			}
		}

		public bool CanNavigateToCell(Vector2Int position)
		{
			if (grid.ContainsCell(position))
			{
				if (navigationRange != null && !navigationRange.Contains(position))
				{
					this.LogWarning($"Restricted navigation range does not contain {position}");
					return false;
				}
				return true;
			}
			return false;
		}
	}
}
