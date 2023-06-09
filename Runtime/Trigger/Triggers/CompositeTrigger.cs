using System.Collections.Generic;

using UnityEngine;

namespace Stratus.Unity.Triggers
{
	/// <summary>
	/// A trigger that is a composition of many other triggers. It will observe a list
	/// of triggers. When all have been activated, so will it be.
	/// </summary>
	public class CompositeTrigger : TriggerBehaviour
	{
		//------------------------------------------------------------------------/
		// Declarations
		//------------------------------------------------------------------------/
		public enum Type { Trigger, Triggerable }
		public enum Criteria { All, Subset }

		//------------------------------------------------------------------------/
		// Fields
		//------------------------------------------------------------------------/
		[Header("Composition")]
		[Tooltip("Whether this trigger is observing trigggers or triggerables")]
		public Type type;
		[Tooltip("The activation criteria")]
		public Criteria criteria;
		[Tooltip("How many elements are needed to activate ")]
		public int needed;
		[Tooltip("The triggers to observe")]
		public TriggerBehaviour[] triggers;
		[Tooltip("The triggerables to observe")]
		public TriggerableBehaviour[] triggerables;

		private List<MonoBehaviour> current = new List<MonoBehaviour>();

		//------------------------------------------------------------------------/
		// Properties
		//------------------------------------------------------------------------/
		/// <summary>
		/// The number of elements set
		/// </summary>
		public int count => type == Type.Trigger ? triggers.Length : triggerables.Length;

		public override string automaticDescription
		{
			get
			{
				return $"On {type}s ({criteria}) being activated";
			}
		}

		//------------------------------------------------------------------------/
		// Messages
		//------------------------------------------------------------------------/
		protected override void OnAwake()
		{
			Compose();
		}

		protected override void OnReset()
		{

		}

		void Compose()
		{
			switch (type)
			{
				case Type.Trigger:
					{
						foreach (var trigger in triggers)
							trigger.onActivate += OnTriggerActivated;
						current.AddRange(triggers);
					}
					break;
				case Type.Triggerable:
					{
						foreach (var triggerable in triggerables)
							triggerable.onTriggered += OnTriggered;
						current.AddRange(triggers);
					}
					break;
				default:
					break;
			}
		}

		void OnTriggerActivated(TriggerBehaviour other)
		{
			current.Remove(other);
			if (ShouldActivate())
				Activate();
		}

		void OnTriggered(TriggerableBehaviour other)
		{
			current.Remove(other);
			if (ShouldActivate())
				Activate();
			StratusDebug.Log("Elements left = " + current.Count, this);
		}

		bool ShouldActivate()
		{
			switch (criteria)
			{
				case Criteria.All:
					if (current.Count == 0)
						return true;
					break;
				case Criteria.Subset:
					if (count - current.Count == needed)
						return true;
					break;
				default:
					break;
			}

			return false;
		}

	}



}