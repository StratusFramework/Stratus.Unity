﻿using Stratus.Unity.Scenes;

using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.SceneManagement;

namespace Stratus.Unity.Triggers
{
	/// <summary>
	/// When triggered, makes sure the selected scenes are loaded
	/// </summary>
	public class SceneLinkTriggerable : TriggerableBehaviour
	{
		//----------------------------------------------------------------------/
		// Fields
		//----------------------------------------------------------------------/
		[Tooltip("What pool of scenes are being considered by this link")]
		public ScenePool scenePool;

		[Tooltip("What scenes should be loaded when this link is triggered.")]
		[HideInInspector]
		public List<SceneField> selectedScenes = new List<SceneField>();

		//----------------------------------------------------------------------/
		// Properties: Public
		//----------------------------------------------------------------------/
		/// <summary>
		/// The last link visited
		/// </summary>
		public static SceneLinkTriggerable lastVisited { private set; get; }
		/// <summary>
		/// All currently enabled links
		/// </summary>
		public static SceneLinkTriggerable[] activeLinks => activeLinksList.ToArray();

		//----------------------------------------------------------------------/
		// Fields: Private
		//----------------------------------------------------------------------/
		private static List<SceneLinkTriggerable> activeLinksList = new List<SceneLinkTriggerable>();

		//----------------------------------------------------------------------/
		// Messages
		//----------------------------------------------------------------------/
		protected override void OnAwake()
		{
			if (SceneLinker.instance == null)
				throw new Exception($"No SceneLinker is available!");
		}

		protected override void OnTrigger(object data = null)
		{
			Load();
		}

		private void OnEnable()
		{
			activeLinksList.Add(this);
		}

		private void OnDisable()
		{
			activeLinksList.Remove(this);
		}

		protected override void OnReset()
		{
		}

		//----------------------------------------------------------------------/
		// Methods
		//----------------------------------------------------------------------/
		/// <summary>
		/// Loads the scenes
		/// </summary>
		public void Load()
		{
			if (lastVisited != this)
			{
				LoadScenes();
				lastVisited = this;
			}
		}

		/// <summary>
		/// Makes sure all scenes specified are loaded when this link is triggered,
		/// unloading all other ones not listed
		/// </summary>
		private void LoadScenes()
		{
			var lut = new Dictionary<string, SceneField>();
			foreach (var scene in selectedScenes)
				lut.Add(scene.name, scene);

			foreach (var scene in StratusScene.activeScenes)
			{
				// Do not unload the master (active scene)
				if (scene.isActiveScene)
					continue;

				if (!lut.ContainsKey(scene.name))
				{
					scene.Unload();
				}
			}

			foreach (var scene in selectedScenes)
			{
				if (!scene.isLoaded)
				{
					if (debug)
						StratusDebug.Log("Loading " + scene);
					scene.Load(LoadSceneMode.Additive);
				}
			}

		}


	}


}