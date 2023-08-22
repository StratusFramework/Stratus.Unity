using Stratus.Unity.Utility;

using System.IO;

using UnityEditor;

using UnityEngine;

namespace Stratus.Unity.Editor
{
	public static class AssetUtility
    {
		/// <summary>
		/// Returns a string of the folder's path that this script is on
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public static string GetFolder(ScriptableObject obj)
		{
			var ms = MonoScript.FromScriptableObject(obj);
			var path = AssetDatabase.GetAssetPath(ms);
			var fi = new FileInfo(path);

			var folder = fi.Directory.ToString();
			folder = folder.Replace('\\', '/');
			return FileUtility.MakeRelative(folder);
		}


		/// <summary>
		/// Adds an instance of the specified ScriptableObject class as a child
		/// to the parent ScriptableObject
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="assetObject"></param>
		/// <returns></returns>
		public static T AddInstanceToAsset<T>(UnityEngine.Object assetObject) where T : ScriptableObject
		{
			var instance = ScriptableObject.CreateInstance<T>();
			instance.hideFlags = HideFlags.HideInHierarchy;
			UnityEditor.AssetDatabase.AddObjectToAsset(instance, assetObject);
			UnityEditor.AssetDatabase.ImportAsset(UnityEditor.AssetDatabase.GetAssetPath(instance));
			UnityEditor.AssetDatabase.SaveAssets();
			UnityEditor.AssetDatabase.Refresh();
			return instance;
		}

	
	}
}
