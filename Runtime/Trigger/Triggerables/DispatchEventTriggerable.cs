using Stratus.Dependencies.TypeReferences;
using Stratus.Models;
using Stratus.Unity.Events;

using System.Collections.Generic;

using UnityEngine;

using Event = Stratus.Events.Event;

namespace Stratus.Unity.Triggers
{
	public class DispatchEventTriggerable : TriggerableBehaviour
	{
		//------------------------------------------------------------------------/
		// Fields
		//------------------------------------------------------------------------/
		[Header("Event")]
		[Tooltip("The scope of the event")]
		public Event.Scope eventScope;
		[DrawIf(nameof(DispatchEventTriggerable.eventScope), Event.Scope.Target, ComparisonType.Equals)]
		[Tooltip("The GameObjects which we want to dispatch the event to")]
		public List<GameObject> targets = new List<GameObject>();
		[ClassExtends(typeof(Event), Grouping = ClassGrouping.ByNamespace)]
		[Tooltip("What type of event this trigger will activate on")]
		public ClassTypeReference type = new ClassTypeReference();

		[SerializeField]
		private string eventData = string.Empty;

		//------------------------------------------------------------------------/
		// Properties
		//------------------------------------------------------------------------/
		public bool hasType => type.Type != null;
		private Event eventInstance { get; set; }

		public override string automaticDescription
		{
			get
			{
				if (hasType)
					return $"Dispatch {type.Type.Name} to {eventScope}";
				return string.Empty;
			}
		}

		//------------------------------------------------------------------------/
		// Messages
		//------------------------------------------------------------------------/
		protected override void OnAwake()
		{
			eventInstance = Event.Instantiate(type, eventData);
		}

		protected override void OnReset()
		{

		}

		protected override void OnTrigger(object data = null)
		{
			switch (eventScope)
			{
				case Event.Scope.Target:
					foreach (var target in targets)
					{
						if (target)
							target.Dispatch(eventInstance, type.Type);
					}
					break;
				case Event.Scope.All:
					StratusScene.Dispatch(eventInstance, type.Type);
					break;
			}
		}

	}

}