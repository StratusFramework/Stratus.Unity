using Stratus.Dependencies.TypeReferences;

using System;

using UnityEngine;
using UnityEngine.Events;

namespace Stratus.Unity.Triggers
{
	/// <summary>
	/// A callback consisting of the Stratus Event received
	/// </summary>
	[Serializable]
	public class UnityStratusEvent : UnityEvent<Events.Event> { }

	public class EventProxy : StratusProxy
	{
		public delegate void OnTriggerMessage(Events.Event e);

		//------------------------------------------------------------------------/
		// Fields
		//------------------------------------------------------------------------/
		[Header("Event")]
		public Events.Event.Scope scope;
		[ClassExtends(typeof(Events.Event), Grouping = ClassGrouping.ByNamespace)]
		[Tooltip("What type of event this trigger will activate on")]
		public ClassTypeReference type = new ClassTypeReference();

		//------------------------------------------------------------------------/
		// Properties
		//------------------------------------------------------------------------/
		/// <summary>
		/// Subscribes to collision events on this proxy
		/// </summary>
		public UnityStratusEvent onTrigger = new UnityStratusEvent();

		public bool hasType => type.Type != null;
		private bool connected { get; set; }

		//------------------------------------------------------------------------/
		// Messages
		//------------------------------------------------------------------------/
		private void Awake()
		{
			if (hasType)
				Subscribe();
		}

		private void Start()
		{
			if (!connected)
				Subscribe();
		}

		//------------------------------------------------------------------------/
		// Methods
		//------------------------------------------------------------------------/
		/// <summary>
		/// Constructs a proxy in order to observe another GameObject's collision messages
		/// </summary>
		/// <param name="target"></param>
		/// <param name="type"></param>
		/// <param name="onCollision"></param>
		/// <param name="persistent"></param>
		/// <returns></returns>
		public static EventProxy Construct(GameObject target, Events.Event.Scope scope, Type type, System.Action<Events.Event> onTrigger, bool persistent = true, bool debug = false)
		{
			var proxy = target.gameObject.AddComponent<EventProxy>();
			proxy.scope = scope;
			proxy.type = type;
			proxy.onTrigger.AddListener(new UnityAction<Events.Event>(onTrigger));
			proxy.persistent = persistent;
			proxy.debug = debug;
			return proxy;
		}

		void OnEvent(Events.Event e)
		{
			if (!e.GetType().Equals(type.Type))
				return;

			//if (debug)
			//  Trace.Script("Triggered by " + type.Type.Name, this);

			onTrigger.Invoke(e);
		}

		private void Subscribe()
		{
			switch (scope)
			{
				case Events.Event.Scope.Target:
					this.gameObject.Connect(this.OnEvent, this.type);
					break;
				case Events.Event.Scope.All:
					StratusScene.Connect(this.OnEvent, this.type);
					break;
			}

			connected = true;
		}


	}

}