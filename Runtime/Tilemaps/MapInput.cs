using Stratus.Logging;
using Stratus.Models.Maps;
using Stratus.Unity.Events;
using Stratus.Unity.Extensions;
using Stratus.Unity.Inputs;

using System;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace Stratus.Unity.Tilemaps
{
	public enum MapInputAction
	{
		Navigate,
		Select,
		Cancel
	}

	public class MapInputActionMapHandler : UnityInputActionMapHandler
	{
		public Action<Vector2> navigate;
		public Action select;
		public Action cancel;

		public MapInputActionMapHandler()
		{
			TryBindAll<MapInputAction>(this);
		}
	}

	public class MapInputLayer : UnityInputLayer<MapInputActionMapHandler>
	{
		public MapInputLayer(string label) : base(label)
		{
		}
	}

	public class MapInput : StratusBehaviour
	{
		[SerializeField]
		private MapManager _map;
		[SerializeField]
		private SpriteRenderer _cursor;

		[Header("Inputs")]
		[SerializeField]
		private InputActionReference select;
		[SerializeField]
		private InputActionReference cancel;
		[SerializeField]
		private InputActionReference navigate;

		public MapManager map => _map;
		public CellSelection currentSelection { get; private set; }
		private MapInputLayer inputLayer { get; set; }

		public event Action<CellSelection> onSelect;
		public event Action<CellSelection> onDeselect;

		private void Awake()
		{
			inputLayer = new MapInputLayer("Map");

			select.action.performed += this.Select;
			cancel.action.performed += this.Deselect;
			navigate.action.performed += this.Navigate;

			map.onLoad += inputLayer.Push;
			map.onUnload += inputLayer.Pop;
		}

		private void Reset()
		{
			_map = GetComponent<MapManager>();
		}

		private void OnEnable()
		{
			select.action.Enable();
		}

		private void OnDisable()
		{
			select.action.Disable();
		}

		private void Select(InputAction.CallbackContext ctx)
		{
			var selection = GetTileAtCurrentMousePosition();
			if (selection != null)
			{
				this.Log($"Selecting {selection}");
				currentSelection = selection;
				UnityEventSystem.Broadcast(new CellSelectionEvent(selection));
			}
		}

		private CellSelection GetTileAtCurrentMousePosition()
		{
			var world = _map.camera.GetMousePositionToWorld();
			return _map.current.SelectFromWorld(world);
		}

		private void Deselect(InputAction.CallbackContext obj)
		{
			this.Log($"Deselect {currentSelection}");
			onDeselect?.Invoke(currentSelection);
			UnityEventSystem.Broadcast(new CellDeselectionEvent(currentSelection));
			currentSelection = null;
		}

		private void Navigate(InputAction.CallbackContext obj)
		{
			var direction = obj.ReadValue<Vector2>().ToVector2Int();
			this.gameObject.Dispatch(new NavigateGridEvent(direction));
			map.Offset(_cursor.transform, direction.ToUnityVector2Int());
		}
	}
}
