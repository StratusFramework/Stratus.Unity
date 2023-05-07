using Stratus.Systems;

using UnityEngine;

namespace Stratus.Unity.Extensions
{
	public class UnityConsoleCommands : IConsoleCommandProvider
	{
		[ConsoleCommand("time")]
		public static float time => Time.realtimeSinceStartup;

		[ConsoleCommand("quit")]
		public static void Quit()
		{
			Application.Quit();
		}
	}
}