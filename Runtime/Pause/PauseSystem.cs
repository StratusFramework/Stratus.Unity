using Stratus.Logging;
using Stratus.Utilities;

using System;

using UnityEngine;
using Stratus.Models.States;
using Stratus.Unity.Scenes;

namespace Stratus.Unity
{
	/// <summary>
	/// A system for pausing the simulation. If not already present,
	/// the instance will be constructed on the scene as a singleton behaviour.
	/// </summary>
	[StratusSingleton(instantiate = true, isPlayerOnly = true)]
	public class PauseSystem : StratusSingletonBehaviour<PauseSystem>
	{
		//------------------------------------------------------------------------/
		// Fields
		//------------------------------------------------------------------------/
		public PauseOptions options = new PauseOptions();

		//------------------------------------------------------------------------/
		// Properties
		//------------------------------------------------------------------------/
		public static bool paused { get; private set; }

		//------------------------------------------------------------------------/
		// Events
		//------------------------------------------------------------------------/
		public static event Action<bool> onPause;

		//------------------------------------------------------------------------/
		// Messages
		//------------------------------------------------------------------------/
		protected override void OnAwake()
		{
			StratusScene.Connect<PauseEvent>(this.OnPauseEvent);
			StratusScene.Connect<ResumeEvent>(this.OnResumeEvent);
		}

		private void OnPauseEvent(PauseEvent e)
		{
			this.SetPause(true);
		}

		private void OnResumeEvent(ResumeEvent e)
		{
			this.SetPause(false);
		}

		//------------------------------------------------------------------------/
		// Static Methods
		//------------------------------------------------------------------------/
		public void Pause(bool pause)
		{
			SetPause(pause);
		}

		public void Toggle()
		{
			SetPause(!paused);
		}

		public void Pause() => Pause(true);
		public void Resume() => Pause(false);

		//------------------------------------------------------------------------/
		// Methods
		//------------------------------------------------------------------------/
		public void Quit()
		{
#if UNITY_EDITOR
			UnityEditor.EditorApplication.ExitPlaymode();
#else
				Application.Quit(0);
#endif
		}

		//------------------------------------------------------------------------/
		// Procedures
		//------------------------------------------------------------------------/
		private void SetPause(bool value)
		{
			if (paused == value)
			{
				return;
			}

			this.Log(value ? $"Pausing..." : "Resuming...");
			onPause?.Invoke(value);
			paused = value;

			if (options.timeScale)
			{
				Time.timeScale = value ? 0f : 1f;
			}
		}
	}
}