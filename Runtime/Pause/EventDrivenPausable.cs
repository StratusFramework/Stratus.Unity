using Stratus.Events;
using Stratus.Models.States;
using Stratus.Unity.Events;
using Stratus.Unity.Scenes;

namespace Stratus.Unity
{
	/// <summary>
	/// Upon receiving specific pause and resume events, pauses the object
	/// </summary>
	/// <typeparam name="PauseEvent"></typeparam>
	/// <typeparam name="ResumeEvent"></typeparam>
	public abstract class EventDrivenPausable<PauseEvent, ResumeEvent>
		: PausableBehaviour
	  where PauseEvent : Event
	  where ResumeEvent : Event
	{
		protected override void SetPauseMechanism()
		{
			StratusScene.Connect<PauseEvent>(this.OnPauseEvent);
			gameObject.Connect<PauseEvent>(this.OnPauseEvent);

			StratusScene.Connect<ResumeEvent>(this.OnResumeEvent);
			gameObject.Connect<ResumeEvent>(this.OnResumeEvent);
		}

		void OnPauseEvent(PauseEvent e)
		{
			this.Pause(true);
		}

		void OnResumeEvent(ResumeEvent e)
		{
			this.Pause(false);
		}
	}

	/// <summary>
	/// Upon receiving the default pause and resume events, pauses the object
	/// </summary>
	public class EventDrivenPausable : EventDrivenPausable<PauseEvent, ResumeEvent>
	{
	}
}