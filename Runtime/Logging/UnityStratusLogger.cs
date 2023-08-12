using Stratus.Logging;

using System;

namespace Stratus.Unity.Logging
{
	public class UnityStratusLogger : StratusLogger
	{
		public override void LogInfo(string message, IStratusLogger logger = null)
		{
			StratusDebug.Log(message, logger);
		}

		public override void LogWarning(string message, IStratusLogger logger = null)
		{
			StratusDebug.LogWarning(message, logger);
		}

		public override void LogError(string message, IStratusLogger logger = null)
		{
			StratusDebug.LogError(message, logger);
		}

		public override void LogException(Exception ex, IStratusLogger logger = null)
		{
			StratusDebug.LogError(ex, logger);
		}
	}
}