using Stratus.Extensions;
using Stratus.Unity.Triggers;

using System.Collections.Generic;

namespace Stratus.Unity.Editor
{
	public partial class TriggerSystemEditor : StratusBehaviourEditor<TriggerSystem>
	{
		/// <summary>
		/// Validates all the components within the system
		/// </summary>
		private void ValidateAll()
		{
			var messages = StratusObjectValidation.Aggregate(target);
			foreach (var msg in messages)
				AddMessage(msg);
		}

		/// <summary>
		/// Validates triggers for persistence
		/// </summary>
		private void ValidatePersistence()
		{
			List<TriggerBase> persistents = new List<TriggerBase>();
			persistents.AddRange(triggers.FindAll(x => x.persistent));
			if (persistents.IsValid())
			{
				string msg = $"Triggers marked as persistent ({persistents.Count}):";
				foreach (var t in persistents)
					msg += $"\n- {t.GetType().Name} : <i>{t.description}</i>";
				AddMessage(new StratusObjectValidation(msg, StratusObjectValidation.Level.Warning, target));
			}
			else
				AddMessage($"There are no persistent triggers in this system", UnityEditor.MessageType.Info, null);
		}

		private void ValidateConnections()
		{
			List<TriggerBase> disconnected = new List<TriggerBase>();
			foreach (var t in triggers)
			{
				if (!IsConnected(t))
					disconnected.Add(t);
			}
			foreach (var t in triggerables)
			{
				if (!IsConnected(t))
					disconnected.Add(t);
			}

			if (disconnected.IsValid())
			{
				string msg = $"Triggers marked as disconnected ({disconnected.Count}):";
				foreach (var t in disconnected)
					msg += $"\n- {t.GetType().Name} : <i>{t.description}</i>";
				AddMessage(msg, UnityEditor.MessageType.Warning, null);
			}
		}

		private void ValidateNull()
		{
			foreach (var t in triggers)
			{
				var validation = StratusUnityObjectValidation.NullReference(t, $"<i>{t.description}</i>");
				if (validation != null) AddMessage(validation);
			}
			foreach (var t in triggerables)
			{
				var validation = StratusUnityObjectValidation.NullReference(t, $"<i>{t.description}</i>");
				if (validation != null) AddMessage(validation);
			}
		}

		/// <summary>
		/// Runs a specific validation function (after some pre-steps)
		/// </summary>
		/// <param name="validateFunc"></param>
		private void Validate(System.Action validateFunc)
		{
			this.messages.Clear();
			validateFunc();
		}

		private TriggerSystem.ConnectionStatus GetStatus(TriggerBehaviour trigger)
		{
			TriggerSystem.ConnectionStatus status = TriggerSystem.ConnectionStatus.Disconnected;
			if (selected)
			{
				if (selected == trigger)
					status = TriggerSystem.ConnectionStatus.Selected;
				else if (selectedTriggerable && connectedTriggers.ContainsKey(trigger) && connectedTriggers[trigger])
					status = TriggerSystem.ConnectionStatus.Connected;
			}

			if (!IsConnected(trigger) && selected != trigger)
				status = TriggerSystem.ConnectionStatus.Disjoint;
			return status;
		}

		private TriggerSystem.ConnectionStatus GetStatus(TriggerableBehaviour triggerable)
		{
			TriggerSystem.ConnectionStatus status = TriggerSystem.ConnectionStatus.Disconnected;
			if (selected)
			{
				if (selected == triggerable)
					status = TriggerSystem.ConnectionStatus.Selected;
				else if (selectedTrigger && connectedTriggerables.ContainsKey(triggerable) && connectedTriggerables[triggerable])
					status = TriggerSystem.ConnectionStatus.Connected;
			}
			if (!IsConnected(triggerable) && selected != triggerable)
				status = TriggerSystem.ConnectionStatus.Disjoint;
			return status;
		}
	}
}