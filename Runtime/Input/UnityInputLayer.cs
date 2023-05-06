﻿using UnityEngine.InputSystem;

namespace Stratus.Inputs
{
	public abstract class UnityInputLayer : InputLayer<InputAction.CallbackContext>
	{
		public UnityInputLayer(string name) : base(name)
		{
		}

	}
}

namespace Stratus.Inputs
{
	public class UnityInputLayer<TActionMap> : UnityInputLayer
		where TActionMap : IActionMapHandler, new()
	{
		public TActionMap actions { get; } = new TActionMap();

		public override bool valid => actions.valid;

		public UnityInputLayer() : this(typeof(TActionMap).Name, new TActionMap())
		{
		}

		public UnityInputLayer(string label) : this(label, new TActionMap())
		{
		}

		public UnityInputLayer(string label, TActionMap actions) : base(label)
		{
			this.actions = actions;
		}

		public override bool HandleInput(InputAction.CallbackContext context)
		{
			return actions.HandleInput(context);
		}

		protected override void OnToggle(bool enabled)
		{
		}
	}
}

namespace Stratus.Inputs
{
	public class DefaultUnityInputLayer : UnityInputLayer
	{
		public UnityInputActionMapHandler actions { get; }
		public override bool valid { get; }

		public DefaultUnityInputLayer(UnityInputActionMapHandler actions)
			 : base(actions.name)
		{
			this.actions = actions;
		}

		public override bool HandleInput(InputAction.CallbackContext context)
		{
			return actions.HandleInput(context);
		}

		protected override void OnToggle(bool enabled)
		{
		}
	}
}