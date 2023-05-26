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
		GridBehaviour grid { get; }
		/// <summary>
		/// The position this behaviour belongs to within a <see cref="Grid"/>
		/// </summary>
		Numerics.Vector2Int cellPosition { get; }
		/// <summary>
		/// The layer this behaviour belongs to
		/// </summary>
		Enumerated layer { get; }

		void Bind(GridBehaviour grid);
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

	public class TileBehaviourAddEvent : TileBehaviourEvent
	{
		public Action<GridBehaviour> initializer { get; private set; }
		public TileBehaviourAddEvent(ITileBehaviour behaviour, Action<GridBehaviour> initializer) : base(behaviour)
		{
			this.initializer = initializer;
		}
	}

	public class TileBehaviourRemovedEvent : TileBehaviourEvent
	{
		public TileBehaviourRemovedEvent(ITileBehaviour behaviour) : base(behaviour)
		{
		}
	}

	public abstract class TileBehaviour : StratusBehaviour, ITileBehaviour, IObject2D
	{
		public GridBehaviour grid { get; protected set; }
		public Numerics.Vector2Int cellPosition { get; protected set; }
		public abstract Enumerated layer { get; }

		public event Action<Numerics.Vector2Int, Numerics.Vector2Int> onMoved;

		public abstract void Select();
		public abstract void Deselect();
		/// <summary>
		/// Binds the given tile behaviour onto the grid it logically belongs to
		/// </summary>
		public abstract void Bind(GridBehaviour grid);
		public abstract void Shutdown();
		public abstract void MoveToPosition(Numerics.Vector2Int targetPosition, bool animate, Action onFinished = null);

		protected void NotifyMoved(Numerics.Vector2Int from, Numerics.Vector2Int to) => onMoved?.Invoke(from, to);
	}

	[RequireComponent(typeof(SpriteRenderer))]
	[RequireComponent(typeof(Collider2D))]

	public abstract class TileBehaviour<TileType> : TileBehaviour, ITileBehaviour<TileType>
		where TileType : StratusTile
	{
		//-------------------------------------------------------------------------//
		// Events
		//-------------------------------------------------------------------------//
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

		//-------------------------------------------------------------------------//
		// Virtual
		//-------------------------------------------------------------------------//
		protected abstract void OnTileBehaviourInitialize();
		protected abstract void OnTileBehaviourShutdown();
		protected abstract void OnMoveToPosition(Numerics.Vector2Int cellPosition);

		//-------------------------------------------------------------------------//
		// Fields
		//-------------------------------------------------------------------------//
		public bool registerAtStart = true;
		public bool destroyOnShutdown = true;
		private bool assignedInitialPosition;

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

		//-------------------------------------------------------------------------//
		// Messages
		//-------------------------------------------------------------------------//
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

		//-------------------------------------------------------------------------//
		// Methods: Initialization
		//-------------------------------------------------------------------------//
		/// <summary>
		/// Registers this behaviour as belonging to a layer within a <see cref="GridBehaviour"/>
		/// </summary>
		protected void Register()
		{
			if (registered)
			{
				return;
			}
			registered = true;
			this.gameObject.DispatchUp(new TileBehaviourAddEvent(this, Bind));
			if (initialized)
			{
				OnTileBehaviourInitialize();
				ToggleVisibility(true);
				this.Log($"Initialized {this} at {cellPosition}");
			}
			else
			{
				this.LogError("Failed to initialize");
			}
		}

		/// <summary>
		/// Removes this behaviour from a layer within a <see cref="GridBehaviour"/>
		/// </summary>
		protected void Unregister()
		{
			if (!registered)
			{
				return;
			}
			registered = false;
			this.gameObject.DispatchUp(new TileBehaviourRemovedEvent(this));
			if (shutdown)
			{
				OnTileBehaviourShutdown();
				ToggleVisibility(false);
				this.Log("Shutdown");
				if (destroyOnShutdown)
				{
					this.Log("Destroying...");
					this.gameObject.Destroy(0.1f);
				}
			}
		}

		public override void Bind(GridBehaviour grid)
		{
			this.grid = grid;
			UpdateInternalPosition();
			MoveToPosition(cellPosition, false);
			FitToCell();
			initialized = true;
		}

		public override void Shutdown()
		{
			if (shutdown)
			{
				return;
			}
			shutdown = true;
		}

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
				cellPosition = grid.behaviour.WorldToCell(this.transform.position).ToNumericVector2Int();
			}
			SnapToPosition(cellPosition);
		}

		public override void MoveToPosition(Numerics.Vector2Int targetPosition, bool animate, Action onFinished = null)
		{
			if (grid.map.grid.Contains(this.layer, targetPosition))
			{
				this.LogWarning($"A behaviour is already present at {targetPosition} in the same layer ({layer}) as this behaviour");
				return;
			}

			this.Log($"Moving agent to {targetPosition}. Animate ? {animate}");
			StartCoroutine(MoveToPositionRoutine(targetPosition, animate, onFinished));
		}

		protected IEnumerator MoveToPositionRoutine(Numerics.Vector2Int targetPosition,
			bool animate, Action onFinished = null)
		{
			var sourcePosition = this.cellPosition;

			// TODO: Implement
			//if (animate)
			//{
			//	// Get the path
			//	Vector3Int[] path = grid.GetPath(sourcePosition, targetPosition);
			//	yield return AnimatedMoveToPosition(path);
			//}
			//else
			//{
			//}
				SnapToPosition(targetPosition);

			yield return new WaitForEndOfFrame();
			this.cellPosition = targetPosition;
			OnMoveToPosition(targetPosition);
			NotifyMoved(sourcePosition, targetPosition);
			onFinished?.Invoke();
		}

		private void SnapToPosition(Numerics.Vector2Int cellPosition)
		{
			Vector3 worldPosition = grid.behaviour.GetCellCenterWorld(cellPosition.ToUnityVector3Int());
			transform.position = worldPosition;
		}

		protected IEnumerator AnimatedMoveToPosition(Numerics.Vector2Int[] path)
		{
			const float defaultTimeBetweenPoints = 0.1f;
			foreach (var point in path)
			{
				Vector3 pointWorldPosition = grid.behaviour.GetCellCenterWorld(point.ToUnityVector3Int());
				//this.Log($"Moving to point {pointWorldPosition}");
				yield return TransformRoutines.MoveTo(transform, pointWorldPosition, defaultTimeBetweenPoints);
			}
		}

		public virtual void FitToCell()
		{
		}

		public override void Select()
		{
			this.Log("Selecting");
			StratusScene.Dispatch<SelectedEvent>(new SelectedEvent(this));
		}

		public override void Deselect()
		{
			this.Log("Deselecting");
			StratusScene.Dispatch<DeselectedEvent>(new DeselectedEvent(this));
		}
	}

	public abstract class StratusTileBehaviour<TBaseTile, TLayer> : TileBehaviour<TBaseTile>
		where TBaseTile : StratusTile
		where TLayer : Enum
	{
		[SerializeField]
		private TLayer _layer = default;
		public override Enumerated layer => _layer.ToString();
	}


}