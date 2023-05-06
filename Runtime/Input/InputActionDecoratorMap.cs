using Stratus.Extensions;

using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.InputSystem;

namespace Stratus.Unity.Inputs
{
	[Serializable]
	public class InputActionDecorator
	{
		public string action;
		public Sprite sprite;
	}

	[CreateAssetMenu(fileName = "Stratus Input Asset Map", menuName = "Stratus/Input/Stratus Input Asset Map")]
	public class InputActionDecoratorMap : StratusScriptable
	{
		[Serializable]
		public class DecoratorCollection
		{
			public StratusInputScheme scheme;
			public List<InputActionDecorator> actions = new List<InputActionDecorator>();

			private Dictionary<string, InputActionDecorator> elementsByLabel { get; set; }

			public InputActionDecorator GetAsset(string label)
			{
				if (elementsByLabel  == null)
				{
					elementsByLabel = new Dictionary<string, InputActionDecorator>();
					elementsByLabel.AddRange(x => x.action, actions);
				}

				if (!elementsByLabel.ContainsKey(label))
				{
					return null;
				}

				return elementsByLabel[label];
			}
		}

		//------------------------------------------------------------------------/
		// Fields
		//------------------------------------------------------------------------/
		public InputActionAsset inputActions;
		public List<DecoratorCollection> schemes = new List<DecoratorCollection>();

		//------------------------------------------------------------------------/
		// Properties
		//------------------------------------------------------------------------/
		public static bool hasGamepad => Input.GetJoystickNames().Length > 0;
		private Dictionary<StratusInputScheme, DecoratorCollection> schemeMap { get; set; }

		//------------------------------------------------------------------------/
		// Methods
		//------------------------------------------------------------------------/
		[InvokeMethod]
		public void Generate()
		{
			schemes = new List<DecoratorCollection>();
		}

		public InputActionDecorator GetDecorator(StratusInputScheme inputScheme, string label)
		{
			if (schemeMap == null)
			{
				schemeMap = new Dictionary<StratusInputScheme, DecoratorCollection>();
				schemeMap.AddRange(x => x.scheme, schemes);
			}

			if (!schemeMap.ContainsKey(inputScheme))
			{
				return null;
			}

			return schemeMap[inputScheme].GetAsset(label);
		}
	}

}