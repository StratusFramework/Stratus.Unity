using Stratus.Logging;
using Stratus.Unity.Events;
using Stratus.Unity.Extensions;

using UnityEngine;
using UnityEngine.InputSystem;

namespace Stratus.Unity.Tilemaps
{
	public class GridInput : StratusBehaviour
    {
		[SerializeField]
		private GridManager _grid;

		[SerializeField]
        private InputActionReference onSelect;
        [SerializeField]
        private InputActionReference onDeselect;
		[SerializeField]
		private InputActionReference onNavigate;
		[SerializeField]
		private InputActionReference onCursor;

		public GridManager grid => _grid;
		public TileSelection currentSelection { get; private set; }
		public TileSelection hovered { get; private set; }

		private void Awake()
		{
			onSelect.action.performed += this.Select;
			onDeselect.action.performed += this.Deselect;
			onNavigate.action.performed += this.Navigate;
			onCursor.action.performed += OnCursor;
		}

		private void Reset()
		{
			_grid = GetComponent<GridManager>();
		}

		private void OnEnable()
		{
			onSelect.action.Enable();			
		}

		private void OnDisable()
		{
			onSelect.action.Disable();
		}

		private void OnCursor(InputAction.CallbackContext ctx)
		{
			if (!ctx.performed)
			{
				return;
			}

			var selection = GetTileAtCurrentMousePosition();
			if (selection != null)
			{
				if (hovered == null || (hovered != null && !hovered.Equals(selection)))
				{
					hovered = selection;
					this.Log($"Cursor moved onto {selection}");
				}
			}
			else
			{
				hovered = null;
			}
		}

		private void Select(InputAction.CallbackContext ctx)
		{
			var selection = GetTileAtCurrentMousePosition();
			if (selection != null)
			{
				this.Log($"Selecting {selection}");
				this.gameObject.Dispatch(new TileSelectionEvent(selection));
			}
		}

		private TileSelection GetTileAtCurrentMousePosition()
		{
			var world = _grid.camera.GetMousePositionToWorld();
			return  _grid.current.SelectFromWorld(world);
		}
		
		private void Deselect(InputAction.CallbackContext obj)
		{
			this.gameObject.Dispatch(new TileDeselectionEvent());
		}

		private void Navigate(InputAction.CallbackContext obj)
		{
			var direction = obj.ReadValue<Vector2>();
			this.gameObject.Dispatch(new NavigateGridEvent(direction));
		}


	}
}
