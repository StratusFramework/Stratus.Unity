﻿using Stratus.Unity.Extensions;

using System;

using UnityEngine;

namespace Stratus.Unity.Behaviours
{
	public class SnapshotBehaviour : StratusBehaviour
	{
		public Camera snapshotCamera;
		public Vector2Int snapshotSize = new Vector2Int(512, 512);
		public int snapshotBits = 16;
		public RenderTexture renderTexture { get; private set; }

		public void Awake()
		{
			CreateRenderTexture();
			snapshotCamera.enabled = false;
			snapshotCamera.targetTexture = renderTexture;
			snapshotCamera.forceIntoRenderTexture = true;
		}

		private void CreateRenderTexture()
		{
			renderTexture = new RenderTexture(snapshotSize.x,
				snapshotSize.y,
				snapshotBits);
			renderTexture.Create();
		}

		public void TakeSnapshot(Action<Texture2D> callback)
		{
			snapshotCamera.Render();
			var routine = renderTexture.GetToTexture2DRoutine(callback);
			StartCoroutine(routine);
		}
	}
}