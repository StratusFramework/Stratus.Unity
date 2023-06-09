﻿using Stratus.Unity.Scenes;
using Stratus.Utilities;

using System.Collections.Generic;

namespace Stratus.Unity.Behaviours
{
	[StratusSingleton(instantiate = true)]
	public class ManagedBehaviourSystem : SingletonBehaviour<ManagedBehaviourSystem>
	{
		//--------------------------------------------------------------------------------------------/
		// Fields
		//--------------------------------------------------------------------------------------------/
		private static List<ManagedBehaviour> behaviours = new List<ManagedBehaviour>();

		//--------------------------------------------------------------------------------------------/
		// Properties
		//--------------------------------------------------------------------------------------------/
		public bool update { get; set; } = true;
		public bool lateUpdate { get; set; } = true;
		public bool fixedUpdate { get; set; } = true;

		//--------------------------------------------------------------------------------------------/
		// Messages
		//--------------------------------------------------------------------------------------------/
		protected override void OnAwake()
		{
		}

		private void Start()
		{
			foreach (ManagedBehaviour behaviour in behaviours)
			{
				behaviour.ManagedStart();
			}
		}

		private void Update()
		{
			if (!update)
				return;

			foreach (ManagedBehaviour behaviour in behaviours)
			{
				if (behaviour.enabled)
				{
					behaviour.ManagedUpdate();
				}
			}
		}

		private void FixedUpdate()
		{
			if (!fixedUpdate)
				return;

			foreach (ManagedBehaviour behaviour in behaviours)
			{
				if (behaviour.enabled)
				{
					behaviour.ManagedFixedUpdate();
				}
			}
		}

		private void LateUpdate()
		{
			if (!lateUpdate)
				return;

			foreach (ManagedBehaviour behaviour in behaviours)
			{
				if (behaviour.enabled)
				{
					behaviour.ManagedLateUpdate();
				}
			}
		}

		//--------------------------------------------------------------------------------------------/
		// Methods
		//--------------------------------------------------------------------------------------------/
		public static void Add(ManagedBehaviour behaviour)
		{
			Instantiate();
			behaviours.Add(behaviour);
		}

		public static void Remove(ManagedBehaviour behaviour)
		{
			behaviours.Remove(behaviour);
		}

		private static void AddCurrentBehaviours()
		{
			ManagedBehaviour[] behaviours = StratusScene.GetComponentsInAllActiveScenes<ManagedBehaviour>();
			StratusDebug.Log($"Adding {behaviours.Length} behaviours");
			ManagedBehaviourSystem.behaviours.AddRange(behaviours);
		}




	}

}