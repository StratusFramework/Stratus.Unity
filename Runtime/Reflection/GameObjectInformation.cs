using Stratus.Extensions;
using Stratus.Logging;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using UnityEngine.Events;

namespace Stratus.Unity.Reflection
{
	/// <summary>
	/// Information about a gameobject and all its components
	/// </summary>
	[Serializable]
	public class GameObjectInformation : ISerializationCallbackReceiver, IStratusLogger
	{
		#region Declarations
		public enum Change
		{
			Components,
			WatchList,
			ComponentsAndWatchList,
			None
		}
		#endregion

		#region Fields
		[SerializeField]
		private string _name;
		[SerializeField]
		private GameObject _gameObject;
		public ComponentInformation[] components;
		public int fieldCount;
		public int propertyCount;
		#endregion

		#region Properties
		public GameObject gameObject => _gameObject;
		public bool initialized { get; private set; }
		public bool debug { get; set; } = true;
		/// <summary>
		/// The recorded number of compoents of the gameobject
		/// </summary>
		public int numberofComponents => components.Length;
		/// <summary>
		/// The components of this GameObject, by their name
		/// </summary>
		public Dictionary<string, List<ComponentInformation>> componentsByName
		{
			get
			{
				if (_componentsByName == null)
				{
					_componentsByName = components.ToDictionaryOfList(c => c.name);
				}
				return _componentsByName;
			}
		}
		private Dictionary<string, List<ComponentInformation>> _componentsByName;

		/// <summary>
		/// The components of this GameObject, by theit type
		/// </summary>
		public Dictionary<Type, List<ComponentInformation>> componentsByType
		{
			get
			{
				if (_componentsByType == null)
				{
					_componentsByType = components.ToDictionaryOfList(c => c.type);
				}
				return _componentsByType;
			}
		}
		private Dictionary<Type, List<ComponentInformation>> _componentsByType;

		public ComponentMemberInfo[] members { get; private set; }
		public ComponentMemberInfo[] visibleMembers { get; private set; }
		public int memberCount => fieldCount + propertyCount;
		public bool isValid => gameObject != null && this.numberofComponents > 0;
		public static UnityAction<GameObjectInformation, Result<Change>> onChanged { get; set; } =
			new UnityAction<GameObjectInformation, Result<Change>>((information, change) => { });
		#endregion

		#region Constants
		public static readonly HashSet<string> hiddenTypeNames = new HashSet<string>()
		{
			typeof(Component).Name
		};
		#endregion

		#region Messages
		public void OnBeforeSerialize()
		{
		}

		public void OnAfterDeserialize()
		{
			if (this.components == null)
			{
				return;
			}
			this.UpdateMemberReferences();
		}

		public override string ToString()
		{
			return gameObject.name;
		}
		#endregion

		#region Constructor
		public GameObjectInformation(GameObject target)
		{
			this._gameObject = target;
			this._name = gameObject.name;
			InitializeComponents(target);
		}
		#endregion

		#region Methods
		/// <summary>
		/// Clears the values of all watch members
		/// </summary>
		public void ClearValues()
		{
			foreach (var component in this.components)
			{
				component.ClearValues();
			}
		}

		/// <summary>
		/// Returns true if the member with a given path (through a component) is present
		/// </summary>
		/// <param name="member"></param>
		/// <returns></returns>
		public bool HasMember(IComponentMemberInfo member)
		{
			return componentsByName.ContainsKey(member.componentName);
		}

		/// <summary>
		/// Clears the values of all watch members
		/// </summary>
		public void ClearWatchValues(ComponentMemberWatchList watchList)
		{
			foreach (var member in watchList.members)
			{
				ClearValue(member);
			}
		}

		/// <summary>
		/// Updates the values in the watch list that are present in this GameObject
		/// </summary>
		public void UpdateValues(ComponentMemberWatchList watchList)
		{
			bool valid = true;
			foreach (var member in watchList.members)
			{
				if (HasMember(member))
				{
					UpdateValue(member);
				}
			}
			if (!valid)
			{
				this.Log("GameObject information is out of date. Refreshing...");
				Refresh();
			}
		}

		/// <summary>
		/// Updates the values of all the members for this GameObject
		/// </summary>
		public void UpdateValues()
		{
			foreach (var component in this.components)
			{
				component.UpdateValues();
			}
		}

		/// <summary>
		/// Clears the values of the given member (if its component is present)
		/// and of any duplicates
		/// </summary>
		public bool ClearValue(IComponentMemberInfo member)
		{
			if (!componentsByName.ContainsKey(member.componentName))
			{
				return false;
			}

			componentsByName[member.componentName].ForEach(c => c.ClearValue(member));
			return true;
		}

		/// <summary>
		/// Updates the values of the given member (if its component is present)
		/// It will also update it for any duplicate components
		/// </summary>
		public bool UpdateValue(ComponentMemberInfo member)
		{
			if (!HasMember(member))
			{
				return false;
			}

			componentsByName[member.componentName].ForEach(c => c.UpdateValue(member));
			return true;
		}

		/// <summary>
		/// Updates the values of the given member (if its component is present)
		/// It will also update it for any duplicate components
		/// </summary>
		public bool UpdateValue(StratusComponentMemberWatchInfo member)
		{
			if (!HasMember(member))
			{
				return false;
			}

			componentsByName[member.componentName].ForEach(c => c.UpdateValue(member));
			return true;
		}

		public bool HasComponent(Type t) => componentsByType.ContainsKey(t);
		public bool HasComponent<T>() where T : Component => HasComponent(typeof(T));

		/// <summary>
		/// Caches all member references from among their components
		/// </summary>
		public void UpdateMemberReferences()
		{
			// Now cache!
			List<ComponentMemberInfo> memberReferences = new List<ComponentMemberInfo>();
			foreach (var component in this.components)
			{
				memberReferences.AddRange(component.memberReferences);
			}
			this.members = memberReferences.ToArray();
			this.visibleMembers = this.members.Where(m => !hiddenTypeNames.Contains(m.typeName)).ToArray();
			this.initialized = true;
		}

		/// <summary>
		/// Refreshes the information for the target GameObject. If any components wwere added or removed,
		/// it will update the cache
		/// </summary>
		public void Refresh()
		{
			var validation = Validate();
			Change change = validation.result;
			if (change != Change.None)
			{
				this.Log($"Changes in {gameObject.name}:\n{validation.message}");
				onChanged(this, validation);
			}

		}
		#endregion

		#region Procedures
		/// <summary>
		/// Initializes the information of the components of this game object
		/// </summary>
		/// <param name="target"></param>
		private void InitializeComponents(GameObject target)
		{
			this.fieldCount = 0;
			this.propertyCount = 0;
			Component[] targetComponents = target.GetComponents<Component>();
			List<ComponentInformation> components = new List<ComponentInformation>();
			for (int i = 0; i < targetComponents.Length; ++i)
			{
				Component component = targetComponents[i];
				if (component == null)
				{
					throw new Exception($"The component at index {i} is null!");
				}

				ComponentInformation componentInfo = new ComponentInformation(component);
				this.fieldCount += componentInfo.fieldCount;
				this.propertyCount += componentInfo.propertyCount;
				components.Add(componentInfo);
			}

			this.components = components.ToArray();
			this.UpdateMemberReferences();
		}

		/// <summary>
		/// Verifies that the component references for this GameObject are still valid
		/// </summary>
		private Result<Change> Validate()
		{
			bool watchlistChanged = false;
			bool changed = false;
			Change change;

			StringBuilder message = new StringBuilder();

			// Update the name if it's changfed
			if (_name != gameObject.name)
			{
				_name = gameObject.name;
				message.AppendLine($"GameObject name changed to {_name}");
			}

			// Check if any components are null
			foreach (ComponentInformation component in this.components)
			{
				if (component.component == null)
				{
					changed = true;
					message.AppendLine($"Component {component.name} is now null");
				}
				else
				{
					if (component.valid)
					{
						changed |= component.Refresh();
						message.AppendLine($"The members of component {component.name} have changed");
					}
				}
			}

			// Check for other component changes
			Component[] actualComponents = gameObject.GetComponents<Component>();
			if (this.numberofComponents != actualComponents.Length)
			{
				changed = true;
				message.AppendLine($"The number of components have changed {numberofComponents} -> {actualComponents.Length}");
			}

			// If there's noticeable changes, let's add any components that were not there before
			if (changed)
			{
				List<ComponentInformation> currentComponents = new List<ComponentInformation>();
				currentComponents.AddRangeWhere((component) => { return component.component != null; },
					this.components);

				// If there's no information for this component, let's add it
				foreach (var component in actualComponents)
				{
					ComponentInformation ci = currentComponents.Find(x => x.component == component);

					if (ci == null)
					{
						ci = new ComponentInformation(component);
						currentComponents.Add(ci);
						message.AppendLine($"Recording new component {ci.name}");
					}
				}

				// Now update the list of components
				this.components = currentComponents.ToArray();
				this.UpdateMemberReferences();
			}

			if (changed)
			{
				if (watchlistChanged)
				{
					change = Change.ComponentsAndWatchList;
				}

				change = Change.Components;
			}
			change = Change.None;

			return new Result<Change>(change == Change.None, change, message.ToString());
		}
		#endregion
	}
}