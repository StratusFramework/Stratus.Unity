﻿using System;

using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Stratus.Unity.Extensions
{
	public static class EventExtensions
	{
		public static void AddListener(this UnityEvent unityEvent, Action action)
		{
			unityEvent.AddListener(() => action());
		}

		public static void AddEventTrigger(this Selectable selectable, EventTriggerType trigger, Action<BaseEventData> onTrigger) 
		{
			var eventTrigger = selectable.gameObject.GetOrAddComponent<EventTrigger>();
			var entry = new EventTrigger.Entry();
			entry.eventID = trigger;
			entry.callback.AddListener((x) => onTrigger(x));
			eventTrigger.triggers.Add(entry);

		}
	}
}