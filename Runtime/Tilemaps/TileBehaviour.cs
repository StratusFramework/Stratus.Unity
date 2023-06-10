using Stratus.Logging;
using Stratus.Models;
using Stratus.Models.Maps;
using Stratus.Unity.Events;
using Stratus.Unity.Extensions;
using Stratus.Unity.Routines;
using Stratus.Unity.Scenes;

using System;
using System.Collections;

using UnityEngine;

namespace Stratus.Unity.Tilemaps
{
	public interface ITileBehaviour
	{
		/// <summary>
		/// The grid this behaviour belongs to
		/// </summary>
		MapBehaviour grid { get; }
		/// <summary>
		/// The position this behaviour belongs to within a <see cref="Grid"/>
		/// </summary>
		Numerics.Vector2Int cellPosition { get; }
		/// <summary>
		/// The layer this behaviour belongs to
		/// </summary>
		Enumerated layer { get; }

		void Bind(MapBehaviour grid);
		void Shutdown();
		void MoveToPosition(Numerics.Vector2Int targetPosition, bool animate, Action onFinished = null);

		void Select();
		void Deselect();
	}

	public interface ITileBehaviour<T> : ITileBehaviour
		where T : StratusTile
	{
	}

	public abstract class TileBehaviourEvent : Stratus.Events.Event
	{
		public ITileBehaviour behaviour { get; private set; }

		protected TileBehaviourEvent(ITileBehaviour behaviour)
		{
			this.behaviour = behaviour;
		}
	}

	/// <summary>
	/// Used by a <see cref="GameObject"/> that is a child of a <see cref="UnityEngine.Tilemaps.Tilemap"/>
	/// </summary>
	public class TileBehaviour : StratusBehaviour, ITileBehaviour, IObject2D
	{
		#region Event Declarations
		public class AddEvent : TileBehaviourEvent
		{
			public Action<MapBehaviour> initializer { get; private set; }
			public AddEvent(ITileBehaviour behaviour, Action<MapBehaviour> initializer) : base(behaviour)
			{
				this.initializer = initializer;
			}
		}

		public class RemoveEvent : TileBehaviourEvent
		{
			public RemoveEvent(ITileBehaviour behaviour) : base(behaviour)
			{
			}
		}

		public class SelectedEvent : TileBehaviourEvent
		{
			public SelectedEvent(ITileBehaviour behaviour) : base(behaviour)
			{
			}
		}

		public class DeselectedEvent : TileBehaviourEvent
		{
			public DeselectedEvent(ITileBehaviour behaviour) : base(behaviour)
			{
			}
		}
		#endregion

		#region Fields
		public bool registerAtStart = true;
		public bool destroyOnShutdown = true;
		private bool assignedInitialPosition;
		#endregion

		#region Properties
		public MapBehaviour grid { get; protected set; }
		public Numerics.Vector2Int cellPosition { get; protected set; }
		public Enumerated layer { get; set; }
		#endregion

		#region Events
		public event Action<Numerics.Vector2Int, Numerics.Vector2Int> onMoved;
		public event Action onInitialize;
		public event Action onShutdown;
		#endregion

		//-------------------------------------------------------------------------//
		// Properties
		//-------------------------------------------------------------------------//
		public SpriteRenderer spriteRenderer => GetComponentCached<SpriteRenderer>();
		/// <summary>
		/// Whether this behaviour has been initialized
		/// </summary>
		public bool initialized { get; private set; }
		/// <summary>
		/// Whether this behaviour was registered
		/// </summary>
		public bool registered { get; private set; }
		/// <summary>
		/// Whether this behaviour has been shutdown
		/// </summary>
		public bool shutdown { get; private set; }

		#region Messages
		private void Start()
		{
			if (registerAtStart)
			{
				Register();
			}
		}

		private void OnDestroy()
		{
			Unregister();
		}

		public override string ToString()
		{
			return $"{name} cell {cellPosition}";
		}
		#endregion

		#region Initialization
		/// <summary>
		/// Registers this behaviour as belonging to a layer within a <see cref="MapBehaviour"/>
		/// </summary>
		protected void Register()
		{
			if (registered)
			{
				return;
			}
			registered = true;
			this.gameObject.DispatchUp(new AddEvent(this, Bind));
			if (initialized)
			{
				onInitialize?.Invoke();
				ToggleVisibility(true);
				this.Log($"Initialized {this} at {cellPosition}");
			}
			else
			{
				this.LogError("Failed to register to a tilemap");
			}
		}

		/// <summary>
		/// Removes this behaviour from a layer within a <see cref="MapBehaviour"/>
		/// </summary>
		protected void Unregister()
		{
			if (!registered)
			{
				return;
			}
			registered = false;
			this.gameObject.DispatchUp(new RemoveEvent(this));
			if (shutdown)
			{
				onShutdown?.Invoke();
				ToggleVisibility(false);
				this.Log("Shutdown");
				if (destroyOnShutdown)
				{
					this.Log("Destroying...");
					this.gameObject.Destroy(0.1f);
				}
			}
		}

		public void Bind(MapBehaviour grid)
		{
			this.grid = grid;
			UpdateInternalPosition();
			MoveToPosition(cellPosition, false);
			FitToCell();
			initialized = true;
		}

		public void Shutdown()
		{
			if (shutdown)
			{
				return;
			}
			shutdown = true;
		}
		#endregion

		#region Interface
		public virtual void MoveToPosition(Numerics.Vector2Int targetPosition, bool animate, Action onFinished = null)
		{
			if (grid.map.grid.Contains(this.layer, targetPosition))
			{
				this.LogWarning($"A behaviour is already present at {targetPosition} in the same layer ({layer}) as this behaviour");
				return;
			}

			this.Log($"Moving agent to {targetPosition}. Animate ? {animate}");
			StartCoroutine(MoveToPositionRoutine(targetPosition, animate, onFinished));
		}
		#endregion

		//-------------------------------------------------------------------------//
		// Methods
		//-------------------------------------------------------------------------//
		protected void ToggleVisibility(bool enabled)
		{
			spriteRenderer.enabled = enabled;
		}

		protected void AssignInitialPosition(Numerics.Vector2Int cellPosition)
		{
			this.cellPosition = cellPosition;
			assignedInitialPosition = true;
		}

		public void UpdateInternalPosition()
		{
			if (!assignedInitialPosition)
			{
				cellPosition = grid.grid.WorldToCell(this.transform.position).ToNumericVector2Int();
			}
			SnapToPosition(cellPosition);
		}

		protected IEnumerator MoveToPositionRoutine(Numerics.Vector2Int targetPosition,
			bool animate, Action onFinished = null)
		{
			var sourcePosition = this.cellPosition;

			// TODO: Implement
			if (animate)
			{
				// Get the path
				//Vector3Int[] path = grid.GetPath(sourcePosition, targetPosition);
				//yield return AnimatedMoveToPosition(path);
			}
			else
			{
				SnapToPosition(targetPosition);
			}

			var oldPosition = cellPosition;
			yield return new WaitForEndOfFrame();
			this.cellPosition = targetPosition;
			NotifyMoved(sourcePosition, targetPosition);
			onFinished?.Invoke();
		}

		private void SnapToPosition(Numerics.Vector2Int cellPosition)
		{
			Vector3 worldPosition = grid.grid.GetCellCenterWorld(cellPosition.ToUnityVector3Int());
			transform.position = worldPosition;
		}

		protected IEnumerator AnimatedMoveToPosition(Numerics.Vector2Int[] path)
		{
			const float defaultTimeBetweenPoints = 0.1f;
			foreach (var point in path)
			{
				Vector3 pointWorldPosition = grid.grid.GetCellCenterWorld(point.ToUnityVector3Int());
				//this.Log($"Moving to point {pointWorldPosition}");
				yield return TransformRoutines.MoveTo(transform, pointWorldPosition, defaultTimeBetweenPoints);
			}
		}

		public virtual void FitToCell()
		{
		}

		public void Select()
		{
			this.Log("Selecting");
			StratusScene.Dispatch<SelectedEvent>(new SelectedEvent(this));
		}

		public void Deselect()
		{
			this.Log("Deselecting");
			StratusScene.Dispatch<DeselectedEvent>(new DeselectedEvent(this));
		}

		protected void NotifyMoved(Numerics.Vector2Int from, Numerics.Vector2Int to) => onMoved?.Invoke(from, to);
	}
}