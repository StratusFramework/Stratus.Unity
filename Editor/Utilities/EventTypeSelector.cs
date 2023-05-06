using Stratus.Events;
using Stratus.Types;

using System;

using UnityEditor;

namespace Stratus.Unity.Editor
{
	/// <summary>
	/// An interface for selecting from Stratus.Events
	/// </summary>
	public class EventTypeSelector : TypeSelector
	{
		//------------------------------------------------------------------------/
		// Fields
		//------------------------------------------------------------------------/
		private SerializedProperty eventDataProperty;
		private Event eventObject;
		private StratusSerializedEditorObject serializedEvent;

		//------------------------------------------------------------------------/
		// CTOR
		//------------------------------------------------------------------------/
		public EventTypeSelector() : base(typeof(Event), false, true)
		{
		}

		private EventTypeSelector(Type baseEventType) : base(baseEventType, false, true)
		{
		}

		public EventTypeSelector Construct<T>() where T : Event
		{
			Type type = typeof(T);
			return new EventTypeSelector(type);
		}

		//------------------------------------------------------------------------/
		// Messages
		//------------------------------------------------------------------------/
		protected override void OnSelectionChanged()
		{
			base.OnSelectionChanged();
			eventObject = (Event)ObjectUtility.Instantiate(selectedClass);
			serializedEvent = new StratusSerializedEditorObject(eventObject);
		}

		//------------------------------------------------------------------------/
		// Methods
		//------------------------------------------------------------------------/
		public void Serialize(SerializedProperty stringProperty)
		{
			this.serializedEvent.Serialize(stringProperty);
		}

		public void EditorGUILayout(SerializedProperty stringProperty)
		{
			if (serializedEvent.DrawEditorGUILayout())
				this.Serialize(stringProperty);
		}
	}
}