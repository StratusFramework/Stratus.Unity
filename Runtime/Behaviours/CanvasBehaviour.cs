﻿using UnityEngine;

namespace Stratus.Unity.Behaviours
{
	[RequireComponent(typeof(Canvas))]
	public class CanvasBehaviour : StratusBehaviour
	{
		[SerializeField]
		private Canvas _canvas;
		[SerializeField]
		private bool _visible = false;

		public Canvas canvas => _canvas;

		public virtual bool visible
		{
			get => _canvas.enabled;
			set
			{
				if (value && !gameObject.activeSelf)
				{
					gameObject.SetActive(true);
				}
				canvas.enabled = value;
			}
		}

		private void Awake()
		{
			visible = _visible;
		}

		private void Reset()
		{
			_canvas = GetComponent<Canvas>();
		}
	}
}