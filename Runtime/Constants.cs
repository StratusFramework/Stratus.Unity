#define STRATUS_CORE

using Stratus.IO;

namespace Stratus.Unity
{
	/// <summary>
	/// Contains information regarding the current modules of the framework.
	/// </summary>
	public static partial class Constants
	{
		public const string rootName = "Stratus";
		public const string rootMenu = rootName + "/Unity/";
		public const string guiFolder = "GUI";
		public const string fontFolder = "Fonts";

		public static string rootPath => StratusIO.GetFolderPath(rootMenu);
		public static string resourcesFolder => rootPath + "/Resources";
		public static string guiPath => resourcesFolder + $"/{guiFolder}";
	}
}