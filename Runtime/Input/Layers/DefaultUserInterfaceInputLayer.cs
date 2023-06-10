﻿using Stratus.Inputs;

using System;

using UnityEngine;
using UnityEngine.InputSystem;

namespace Stratus.Unity.Inputs
{
	public enum DefaultUserInterfaceAction
	{
		Navigate,
		Submit,
		Cancel,
	}

	/// <summary>
	/// Default input action map for UI
	/// </summary>
	public class DefaultUserInterfaceActionMap : ActionMapHandlerBase<InputAction.CallbackContext>
	{
		public override string name => "UI";

		public override bool valid { get; }

		public Action<Vector2> onNavigate;
		public Action onSubmit;
		public Action onCancel;
		public Action onNext;
		public Action onPrevious;
		public Action onReset;
		public Action onPause;

		public const string navigationActionName = nameof(DefaultUserInterfaceAction.Navigate);// "Navigate";
		public const string submitActionName = nameof(DefaultUserInterfaceAction.Submit);
		public const string cancelActionName = nameof(DefaultUserInterfaceAction.Cancel);

		public override bool HandleInput(InputAction.CallbackContext context)
		{
			bool handled = false;
			if (context.performed)
			{
				switch (context.action.name)
				{
					case navigationActionName:
						onNavigate?.Invoke(context.ReadValue<Vector2>());
						handled = true;
						break;

					case submitActionName:
						onSubmit?.Invoke();
						handled = true;
						break;

					case cancelActionName:
						onCancel?.Invoke();
						handled = true;
						break;

				}
			}
			return handled;
		}
	}

	public class DefaultUserInterfaceInputLayer : UnityInputLayer<DefaultUserInterfaceActionMap>
	{
		public DefaultUserInterfaceInputLayer(string label) : base(label)
		{
		}

		public DefaultUserInterfaceInputLayer(string label, DefaultUserInterfaceActionMap actions) : base(label, actions)
		{
		}


		protected override void OnToggle(bool enabled)
		{
			base.OnToggle(enabled);
			if (active)
			{
				CursorLock.ReleaseLock();
			}
			else
			{
				CursorLock.RevertLock();
			}
		}
	}
}