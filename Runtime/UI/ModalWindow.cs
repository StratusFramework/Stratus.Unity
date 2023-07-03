using Stratus.Extensions;
using Stratus.Models;
using Stratus.Unity.Events;
using Stratus.Unity.Inputs;

using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine.UIElements;

namespace Stratus.Unity.UI
{
	/// <summary>
	/// A singleton for displaying a modal window, at any time!
	/// </summary>
	public class ModalWindow : DocumentBehaviour
	{
		public class OpenEvent : Stratus.Events.Event
		{
			public string content;
			public LabeledAction[] actions;
		}

		private DefaultUserInterfaceInputLayer inputLayer = new DefaultUserInterfaceInputLayer();
		private ScrollView buttons;
		private Label message;

		public bool open { get; private set; }

		private void Awake()
		{
			UnityEventSystem.Connect<OpenEvent>(Open);

			var background = root.Q("Background");
			background.RegisterCallback<PointerDownEvent>(e =>
			{
				e.StopPropagation();
			});
			var frame = root.Q("Frame");
			message = frame.Q<Label>("Message");
			buttons = frame.Q<ScrollView>("Buttons");

			visible = false;
		}

		public void Open(OpenEvent e)
		{
			if (open)
			{
				throw new Exception("Modal window already open");
			}

			open = true;
			visible = true;
			inputLayer.Push();
			message.text = e.content;

			foreach(var action in e.actions)
			{
				buttons.Add(new Button(action.action.Append(Close))
				{
					text = action.label
				});
			}

			buttons.Children().First().Focus();
		}

		private void Close()
		{
			open = false;
			inputLayer.Pop();
			buttons.Clear();
			visible = false;
		}

		public static void Confirm(string message, Action onConfirm, Action onCancel = null)
			=> UnityEventSystem.Broadcast(new OpenEvent()
			{
				content = message,
				actions = new LabeledAction[]
				{
					new LabeledAction("Confirm", onConfirm),
					new LabeledAction("Cancel", onCancel),				}
			});
	}
}
