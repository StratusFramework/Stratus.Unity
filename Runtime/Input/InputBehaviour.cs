﻿using UnityEngine;

namespace Stratus.Unity.Inputs
{
	public abstract class InputBehaviour : StratusBehaviour
	{
		//------------------------------------------------------------------------/
		// Fields
		//------------------------------------------------------------------------/
		[SerializeField]
		private StratusInputScheme _inputScheme = StratusInputScheme.KeyboardMouse;
		public bool active = true;

		//------------------------------------------------------------------------/
		// Properties
		//------------------------------------------------------------------------/
		public StratusInputScheme inputScheme
		{
			get => _inputScheme;
			set
			{
				if (_inputScheme != value)
				{
					_inputScheme = value;

				}
			}
		}
		public Vector2 mousePosition => Input.mousePosition;

		//------------------------------------------------------------------------/
		// Messages
		//------------------------------------------------------------------------/
		protected abstract void PollInput(StratusInputScheme inputScheme);
		protected abstract void OnInputSchemeChanged(StratusInputScheme inputScheme);

		//------------------------------------------------------------------------/
		// Messages
		//------------------------------------------------------------------------/
		private void Update()
		{
			if (active)
			{
				PollInput(inputScheme);
			}
		}
	}
}