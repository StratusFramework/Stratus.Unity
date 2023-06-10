using Stratus.Logging;
using Stratus.Models.Maps;
using Stratus.Unity.Events;
using Stratus.Unity.Extensions;
using Stratus.Unity.Inputs;

using System;

using UnityEngine;
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

		[SerializeField]
		private InputActionReference onSelect;
		[SerializeField]
		private InputActionReference onCancel;
		[SerializeField]
		private InputActionReference onNavigate;

		public MapManager map => _map;
		public TileSelection currentSelection { get; private set; }

		private MapInputLayer inputLayer;

		private void Awake()
		{
			inputLayer = new MapInputLayer("Map");

			onSelect.action.performed += this.Select;
			onCancel.action.performed += this.Deselect;
			onNavigate.action.performed += this.Navigate;

			map.onLoad += inputLayer.Push;
			map.onUnload += inputLayer.Pop;
		}

		private void Reset()
		{
			_map = GetComponent<MapManager>();
		}

		private void OnEnable()
		{
			onSelect.action.Enable();
		}

		private void OnDisable()
		{
			onSelect.action.Disable();
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
			var world = _map.camera.GetMousePositionToWorld();
			return _map.current.SelectFromWorld(world);
		}

		private void Deselect(InputAction.CallbackContext obj)
		{
			this.gameObject.Dispatch(new TileDeselectionEvent());
		}

		private void Navigate(InputAction.CallbackContext obj)
		{
			var direction = obj.ReadValue<Vector2>().ToVector2Int();
			this.gameObject.Dispatch(new NavigateGridEvent(direction));
			map.Offset(_cursor.transform, direction.ToUnityVector2Int());
		}
	}
}
