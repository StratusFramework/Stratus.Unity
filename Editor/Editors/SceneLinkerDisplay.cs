﻿using Stratus.Models;
using Stratus.Unity.Rendering;
using Stratus.Unity.Scenes;
using Stratus.Unity.Triggers;

using UnityEditor;

using UnityEngine;

namespace Stratus.Unity.Editor
{
	[LayoutViewDisplayAttribute("Scene Linker", 225f, 200f, Anchor.TopLeft, Dimensions.Absolute)]
	public class SceneLinkerDisplay : SingletonSceneViewDisplay<SceneLinker>
	{
		private Vector2 displayScrollPosition { get; set; } = Vector2.zero;
		private float displayOpenButtonWidth => 50f;
		private Color displayLinksColor => Color.yellow;
		private Color displayBoundariesColor => Color.gray;

		private SceneLinkTriggerable[] sceneLinks;
		private Bounds[] sceneBoundaries;

		protected override bool isValid
		{
			get
			{
				return base.isValid && instance.showDisplay;
			}
		}

		protected override void OnInitializeSingletonState()
		{
			this.sceneLinks = StratusScene.GetComponentsInAllActiveScenes<SceneLinkTriggerable>();
			int numScenes = StratusScene.activeScenes.Length;
			this.sceneBoundaries = new Bounds[numScenes];
			for (int i = 0; i < numScenes; ++i)
			{
				this.sceneBoundaries[i] = StratusScene.activeScenes[i].visibleBoundaries;
			}
		}

		protected override void OnGUI(Rect position)
		{
			if (instance.displayLinks)
			{
				Handles.color = displayLinksColor;
				foreach (var link in sceneLinks)
				{
					if (link == null)
						continue;

					var linkPos = link.transform.position;
					Handles.DrawWireCube(linkPos, link.transform.localScale);
				}
			}

			if (instance.displayBoundaries)
			{
				Handles.color = displayBoundariesColor;
				foreach (var bounds in sceneBoundaries)
				{
					Handles.DrawWireCube(bounds.center, bounds.size);
				}
			}
		}

		protected override void OnGUILayout(Rect position)
		{
			if (instance.scenePool == null)
			{
				GUILayout.Label("No ScenePool asset has been set!");
				return;
			}

			GUILayout.BeginHorizontal();
			GUILayout.Label("Scenes:");
			if (GUILayout.Button("Open All")) SceneLinker.OpenAll();
			if (GUILayout.Button("Close All")) SceneLinker.CloseAll();
			GUILayout.EndHorizontal();
			GUILayout.Space(2.5f);

			displayScrollPosition = GUILayout.BeginScrollView(displayScrollPosition, false, false);
			foreach (var scene in instance.scenes)
			{
				GUILayout.BeginHorizontal();
				GUILayout.Label($"- " + scene.name);
				if (scene.isOpened)
				{
					if (GUILayout.Button("Close", EditorStyles.miniButtonRight, GUILayout.Width(displayOpenButtonWidth)))
						scene.Close();
				}
				else
				{
					if (GUILayout.Button("Open", EditorStyles.miniButtonRight, GUILayout.Width(displayOpenButtonWidth)))
						scene.Add();
				}
				GUILayout.EndHorizontal();
			}
			GUILayout.EndScrollView();
			GUILayout.BeginHorizontal();
			{
				GUILayout.Label($"Links ({sceneLinks.Length})");
				StratusEditorGUILayout.Toggle(instance, "displayLinks", "Show");
				if (GUILayout.Button("Select"))
				{
					foreach (var link in sceneLinks)
					{
						EditorGUIUtility.PingObject(link);
					}

				}
			}
			GUILayout.EndHorizontal();

			StratusEditorGUILayout.Toggle(instance, "displayBoundaries", "Show scene boundaries");
			StratusEditorGUILayout.Toggle(instance, "loadInitial", "Load initial scene on play");

		}



	}

}