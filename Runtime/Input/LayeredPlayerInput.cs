using Stratus.Inputs;
using Stratus.Logging;
using Stratus.Unity.Behaviours;
using Stratus.Unity.Rendering;
using Stratus.Unity.Scenes;
using Stratus.Utilities;

using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.InputSystem;

namespace Stratus.Unity.Inputs
{
	/// <summary>
	/// Base class for inputs that work with Unity's newer InputSystem
	/// </summary>
	[StratusSingleton(instantiate = false)]
	public abstract class LayeredPlayerInput<T> : SingletonBehaviour<T>, IStratusPlayerInput
		where T : SingletonBehaviour<T>
	{
		#region Fields
		public PlayerInput playerInput;
		public InputActionDecoratorMap decoratorMap;
		public bool debug = false;
		public bool logInputCallback = false;

		private InputStack<InputLayer> inputLayers = new InputStack<InputLayer>();
		private Dictionary<string, StratusInputScheme> inputSchemes = new Dictionary<string, StratusInputScheme>();
		#endregion

		#region Properties
		public InputActionMap currentActionMap => playerInput.currentActionMap;
		public InputLayer currentInputLayer => inputLayers.current;
		public string currentActionMapName => playerInput.currentActionMap.name;
		public bool hasInputLayer => inputLayers.count > 0;
		public bool inputEnabled
		{
			get => playerInput != null ? playerInput.enabled : false;
			set
			{
				if (playerInput != null)
				{
					if (debug)
					{
						this.Log($"{(value ? "Enabling" : "Disabling")} player input");
					}
					playerInput.enabled = value;
				}
			}
		}
		public StratusInputScheme latestInputSchemeUsed { get; private set; }
		#endregion

		#region Events
		public event Action<StratusInputScheme> onInputSchemeChanged;
		#endregion

		#region Virtual
		protected abstract void OnInputAwake();
		protected abstract void OnInputSchemeChanged(StratusInputScheme inputScheme);
		#endregion

		#region Messages
		protected override void OnAwake()
		{
			StratusScene.Connect<InputLayer.PushEvent>(OnPushLayerEvent);
			StratusScene.Connect<InputLayer.PopEvent>(OnPopLayerEvent);
			if (playerInput.notificationBehavior != PlayerNotifications.InvokeCSharpEvents)
			{
				throw new Exception($"The player input must be set to {PlayerNotifications.InvokeCSharpEvents}");
			}
			playerInput.onActionTriggered += OnInputActionTriggered;
			playerInput.onControlsChanged += this.OnControlsChanged;
			inputLayers.onLayerToggled += this.OnInputLayerChanged;
			RecordInputDevices();
			OnInputAwake();
		}

		private void OnControlsChanged(PlayerInput obj)
		{
		}

		private void Reset()
		{
			playerInput = GetComponent<PlayerInput>();
		}

		private void OnGUI()
		{
			if (debug)
			{
				if (inputLayers.hasLayers)
				{
					StratusGUI.GUILayoutArea(Anchor.TopRight, StratusGUI.quarterScreen, (Rect rect) =>
					{
						GUILayout.Label(inputLayers.current.name, Styles.headerWhite);
					});
				}
			}
		}
		#endregion

		#region Event Handlers
		private void OnPushLayerEvent(InputLayer.PushEvent e)
		{
			Result result = inputLayers.Push(e.layer);
			if (debug)
			{
				this.Log($"Pushed input layer: {e.layer.name}. Result ? {result}");
			}
		}

		private void OnPopLayerEvent(InputLayer.PopEvent e)
		{
			// Pop layers that have been marked as inactive
			inputLayers.TryPop(e.layer);

			//while (inputLayers.canPop)
			//{
			//	InputLayer layer = inputLayers.Pop();
			//	if (debug)
			//	{
			//		if (layer != null)
			//		{
			//			this.Log($"Removed input layer: {layer}. Current layer: {inputLayers.activeLayer}");
			//		}
			//		else
			//		{
			//			this.LogError("No input layer to remove!");
			//		}
			//	}
			//}
		}

		private void OnInputLayerChanged(InputLayer layer)
		{
			bool switched = false;

			if (!IsCurrentActionMap(layer.name))
			{
				try
				{
					if (!playerInput.enabled)
					{
						playerInput.enabled = true;
					}
					playerInput.SwitchCurrentActionMap(layer.name);
					if (IsCurrentActionMap(layer.name))
					{
						switched = true;
					}
					else
					{
						this.LogError($"Could not find action map for layer {layer.name}");
					}
				}
				catch (Exception exception)
				{
					this.LogException(exception);
				}
			}

			if (debug)
			{
				if (switched)
				{
					this.Log($"Action map switched to {layer.name}");
				}
				this.Log($"Input layer now '{layer.name}'");
			}
		}

		private bool IsCurrentActionMap(string actionMapName)
		{
			return playerInput.currentActionMap != null && playerInput.currentActionMap.name.Equals(actionMapName, StringComparison.InvariantCultureIgnoreCase);
		}
		#endregion

		#region Methods
		public void ActivateInput()
		{
			playerInput.ActivateInput();
		}

		public void DeactivateInput()
		{
			playerInput.DeactivateInput();
		}

		public InputActionDecorator GetActionDecorator(string action)
		{
			if (decoratorMap == null)
			{
				this.LogWarning("No decorator asset has been assigned");
				return null;
			}

			return decoratorMap.GetDecorator(this.latestInputSchemeUsed, action);
		}

		public Sprite GetActionSprite(string action)
		{
			var decorator = GetActionDecorator(action);
			if (decorator == null)
			{
				return null;
			}
			return decorator.sprite;
		}
		#endregion

		#region Static Methods
		public static void DispatchPushLayerEvent(UnityInputLayer layer)
		{
			StratusScene.Dispatch(new UnityInputLayer.PushEvent(layer));
		}

		public static void DispatchPopLayerEvent(UnityInputLayer layer)
		{
			StratusScene.Dispatch(new UnityInputLayer.PopEvent(layer));
		}

		public static StratusInputScheme TryParse(string deviceName)
		{
			StratusInputScheme result = StratusInputScheme.Unknown;
			switch (deviceName)
			{
				case "Keyboard":
					result = StratusInputScheme.KeyboardMouse;
					break;

				case "Mouse":
					result = StratusInputScheme.KeyboardMouse;
					break;
			}
			if (result == StratusInputScheme.Unknown)
			{
				if (deviceName.Contains("DualShock"))
				{
					result = StratusInputScheme.DualShock;
				}
			}
			return result;
		}

		public static void SwitchMap(string name)
		{
		}
		#endregion

		#region Procedures
		private void OnInputActionTriggered(InputAction.CallbackContext context)
		{
			if (hasInputLayer)
			{
				// Update the current input device if it has changed
				string deviceName = context.control.device.name;
				if (inputSchemes.ContainsKey(deviceName))
				{
					var device = inputSchemes[deviceName];
					if (latestInputSchemeUsed != device)
					{
						UpdateLatestInputDevice(device);
					}
				}

				// Handle the input
				if (logInputCallback)
				{
					bool handled = currentInputLayer.HandleInput(context);
					this.Log($"[{(handled ? "HANDLED" : "UNHANDLED")}] {context}");
				}
				else
				{
					currentInputLayer.HandleInput(context);
				}
			}
			else
			{
				if (logInputCallback)
				{
					this.LogWarning($"[NO INPUT LAYER] {context}");
				}
			}
		}

		private void RecordInputDevices()
		{
			foreach (InputDevice device in playerInput.devices)
			{
				StratusInputScheme result = TryParse(device.name);
				if (result != StratusInputScheme.Unknown)
				{
					inputSchemes.Add(device.name, result);
				}
				if (debug)
				{
					this.Log($"Device '{device.name}' detected as {result}");
				}
			}
		}

		private void UpdateLatestInputDevice(StratusInputScheme inputScheme)
		{
			latestInputSchemeUsed = inputScheme;
			onInputSchemeChanged?.Invoke(latestInputSchemeUsed);
			OnInputSchemeChanged(latestInputSchemeUsed);
			if (debug)
			{
				this.Log($"Last device used now {latestInputSchemeUsed}");
			}
		}
		#endregion
	}

	public class LayeredPlayerInput : LayeredPlayerInput<LayeredPlayerInput>
	{
		protected override void OnInputAwake()
		{
		}

		protected override void OnInputSchemeChanged(StratusInputScheme inputScheme)
		{
		}
	}

	public static class LayeredPlayerInputExtensions
	{
		public static void Push(this UnityInputLayer layer) => LayeredPlayerInput.DispatchPushLayerEvent(layer);
		public static void Pop(this UnityInputLayer layer) => LayeredPlayerInput.DispatchPopLayerEvent(layer);
	}

	public enum StratusInputScheme
	{
		Unknown,
		KeyboardMouse,
		DualShock,
		Xbox
	}

	public interface IStratusPlayerInput
	{
		event Action<StratusInputScheme> onInputSchemeChanged;
		InputActionDecorator GetActionDecorator(string action);
	}

}