using Stratus.Inputs;

using UnityEngine.InputSystem;

namespace Stratus.Unity.Inputs
{
	public abstract class UnityInputLayer : InputLayer<InputAction.CallbackContext>
	{
		public UnityInputLayer(string name) : base(name)
		{
		}

	}

	public class UnityInputLayer<TActionMap> : UnityInputLayer
		where TActionMap : IActionMapHandler, new()
	{
		public TActionMap actions { get; } = new TActionMap();

		public override bool valid => actions.valid;

		public UnityInputLayer() : this(typeof(TActionMap).Name, new TActionMap())
		{
		}

		public UnityInputLayer(string name) : this(name, new TActionMap())
		{
		}

		public UnityInputLayer(string name, TActionMap actions) : base(name)
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