using UnityEngine;

namespace Stratus.Unity.Routines
{
	public class WaitForUnscaledUpdate : CustomYieldInstruction
	{
		private float WaitTIme;

		public override bool keepWaiting
		{
			get
			{
				return Time.realtimeSinceStartup < WaitTIme;
			}
		}

		public WaitForUnscaledUpdate()
		{
			WaitTIme = Time.realtimeSinceStartup + Time.fixedDeltaTime;
		}
	}
}
