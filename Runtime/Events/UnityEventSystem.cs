﻿using Stratus.Events;
using Stratus.Unity.Extensions;

using System;
using System.Collections;

using UnityEngine;

using Event = Stratus.Events.Event;

namespace Stratus.Unity.Events
{
	public class UnityEventSystem : EventSystem<GameObject>
	{
		protected override void OnConnect(GameObject obj)
		{
			base.OnConnect(obj);
			obj.AddComponent<EventsRegistration>();
		}

		protected override bool IsNull(object obj)
		{
			if (obj == null || obj is UnityEngine.Object & obj.Equals(null))
			{
				return true;
			}

			return false;
		}

		/// <summary>
		/// Dispatches the given event of the specified type onto the GameObject amd all its children.
		/// </summary>
		/// <typeparam name="T">The event class. </typeparam>
		/// <param name="gameObj">The GameObject to which to dispatch to.</param>
		/// <param name="eventObj">The event object. </param>
		public static void DispatchDown<T>(GameObject gameObj, T eventObj) where T : Event
		{
			foreach (GameObject child in gameObj.Children())
			{
				Dispatch<T>(child, eventObj);
			}
		}

		/// <summary>
		/// Dispatches an event up the tree on each parent recursively.
		/// </summary>
		/// <typeparam name="T">The event class. </typeparam>
		/// <param name="gameObj">The GameObject to which to dispatch to.</param>
		/// <param name="eventObj">The event object. </param>
		public static void DispatchUp<T>(GameObject gameObj, T eventObj) where T : Event
		{
			Transform[] parents = gameObj.transform.GetComponentsInParent<Transform>();
			foreach (Transform parent in parents)
			{
				Dispatch<T>(parent.gameObject, eventObj);
			}
		}

		/// <summary>
		/// Dispatches the event on the next frame.
		/// </summary>
		/// <typeparam name="T">The event class.</typeparam>
		/// <param name="obj">The object to which to dispatch to.</param>
		/// <param name="eventObj">The event object we are sending.</param>
		/// <returns></returns>
		public static IEnumerator DispatchNextFrame<T>(GameObject obj, T eventObj) where T : Event
		{
			// Wait 1 frame
			yield return 0;
			// Dispatch the event
			Dispatch<T>(obj, eventObj);
		}

		/// <summary>
		/// Dispatches the event on the next frame.
		/// </summary>
		/// <typeparam name="T">The event class.</typeparam>
		/// <param name="obj">The object to which to dispatch to.</param>
		/// <param name="eventObj">The event object we are sending.</param>
		/// <returns></returns>
		public static IEnumerator DispatchNextFrame(GameObject obj, Event eventObj, Type type)
		{
			// Wait 1 frame
			yield return 0;
			// Dispatch the event
			Dispatch(obj, eventObj);
		}
	}
}