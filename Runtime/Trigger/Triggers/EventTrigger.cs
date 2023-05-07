using Stratus.Dependencies.TypeReferences;
using Stratus.Models;

using UnityEngine;

using Event = Stratus.Events.Event;

namespace Stratus.Unity.Triggers
{
	/// <summary>
	/// A trigger that is activated when a specified stratus event is received.
	/// </summary>
	public class EventTrigger : TriggerBehaviour
	{
		//------------------------------------------------------------------------/
		// Fields
		//------------------------------------------------------------------------/
		[Header("Event")]
		[Tooltip("The scope of the event")]
		public Event.Scope eventScope;
		[DrawIf(nameof(EventTrigger.eventScope), Event.Scope.Target, ComparisonType.Equals)]
		[Tooltip("The source GameObject which we want to listen the event on")]
		public GameObject source;
		[ClassExtends(typeof(Event), Grouping = ClassGrouping.ByNamespace)]
		[Tooltip("What type of event this trigger will activate on")]
		public ClassTypeReference type;

		//[SerializeField]
		//private string eventData;

		//------------------------------------------------------------------------/
		// Properties
		//------------------------------------------------------------------------/
		public override string automaticDescription
		{
			get
			{
				if (hasType)
					return $"On {type.Type.Name}";
				return string.Empty;
			}
		}

		public EventProxy proxy { get; private set; }
		public bool hasType => type.Type != null;

		//------------------------------------------------------------------------/
		// Messages
		//------------------------------------------------------------------------/
		protected override void OnAwake()
		{
			if (type.Type == null)
			{
				StratusDebug.LogError("Type not set. Please select the Stratus.Event type to connect to!", this);
				return;
			}

			proxy = EventProxy.Construct(source, eventScope, type, OnEvent, persistent, debug);

		}

		void OnEvent<T>(T e) where T : Event
		{
			Activate();
			//Trace.Script($"Triggered on {e.GetType()}");
		}


		protected override void OnReset()
		{
			source = this.gameObject;
		}


	}
}