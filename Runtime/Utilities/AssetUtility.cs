using Stratus.Extensions;

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using UnityEngine;

namespace Stratus.Unity.Utility
{
	/// <summary>
	/// Provides utility methods for dealing with Unity assets at runtime
	/// </summary>
	public static class AssetUtility
	{
		#region Assets
		/// <summary>
		/// The symbol used to separate folders by Unity's API
		/// </summary>
		public const char UnityDirectorySeparator = '/';

		/// <summary>
		/// The name of the resources folder
		/// </summary>
		public const string ResourcesFolderName = "Resources";

		/// <summary>
		/// Returns a string of the folder's path that this script is on
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public static string GetFolder(ScriptableObject obj)
		{
#if UNITY_EDITOR
			var ms = UnityEditor.MonoScript.FromScriptableObject(obj);
			var path = UnityEditor.AssetDatabase.GetAssetPath(ms);
			var fi = new FileInfo(path);

			var folder = fi.Directory.ToString();
			folder = folder.Replace('\\', '/');
			return FileUtility.MakeRelative(folder);
#else
      return string.Empty;
#endif
		}

		/// <summary>
		/// Creates the asset and any directories that are missing along its path.
		/// </summary>
		/// <param name="unityObject">UnityObject to create an asset for.</param>
		/// <param name="unityFilePath">Unity file path (e.g. "Assets/Resources/MyFile.asset".</param>
		public static void CreateAssetAndDirectories(UnityEngine.Object unityObject, string unityFilePath)
		{
#if UNITY_EDITOR
			var dir = Path.GetDirectoryName(unityFilePath);
			var pathDirectory = dir + UnityDirectorySeparator;

			bool hasFolder = UnityEditor.AssetDatabase.IsValidFolder(dir);
			if (!hasFolder)
				CreateDirectoriesInPath(pathDirectory);

			UnityEditor.AssetDatabase.CreateAsset(unityObject, unityFilePath);
#endif
		}


		private static void CreateDirectoriesInPath(string unityDirectoryPath)
		{
#if UNITY_EDITOR
			// Check that last character is a directory separator
			if (unityDirectoryPath[unityDirectoryPath.Length - 1] != UnityDirectorySeparator)
			{
				var warningMessage = string.Format(
										 "Path supplied to CreateDirectoriesInPath that does not include a DirectorySeparator " +
										 "as the last character." +
										 "\nSupplied Path: {0}, Filename: {1}",
										 unityDirectoryPath);
				Debug.LogWarning(warningMessage);
			}

			// Warn and strip filenames
			var filename = Path.GetFileName(unityDirectoryPath);
			if (!string.IsNullOrEmpty(filename))
			{
				var warningMessage = string.Format(
										 "Path supplied to CreateDirectoriesInPath that appears to include a filename. It will be " +
										 "stripped. A path that ends with a DirectorySeparate should be supplied. " +
										 "\nSupplied Path: {0}, Filename: {1}",
										 unityDirectoryPath,
										 filename);
				Debug.LogWarning(warningMessage);

				unityDirectoryPath = unityDirectoryPath.Replace(filename, string.Empty);
			}

			var folders = unityDirectoryPath.Split(UnityDirectorySeparator);

			// Error if path does NOT start from Assets
			if (folders.Length > 0 && folders[0] != "Assets")
			{
				var exceptionMessage = "AssetDatabaseUtility CreateDirectoriesInPath expects full Unity path, including 'Assets\\\". " +
									   "Adding Assets to path.";
				throw new UnityException(exceptionMessage);
			}

			string pathToFolder = string.Empty;
			foreach (var folder in folders)
			{
				// Don't check for or create empty folders
				if (string.IsNullOrEmpty(folder))
				{
					continue;
				}

				// Create folders that don't exist
				pathToFolder = string.Concat(pathToFolder, folder);
				if (!UnityEditor.AssetDatabase.IsValidFolder(pathToFolder))
				{
					var pathToParent = Directory.GetParent(pathToFolder).ToString();
					UnityEditor.AssetDatabase.CreateFolder(pathToParent, folder);
					UnityEditor.AssetDatabase.Refresh();
				}

				pathToFolder = string.Concat(pathToFolder, UnityDirectorySeparator);
			}
#endif
		}

		/// <summary>
		/// Gets the asset of type T at the given path
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="path"></param>
		/// <returns></returns>
		public static T[] GetAtPath<T>(string path)
		{
#if UNITY_EDITOR
			ArrayList al = new ArrayList();
			string[] fileEntries = Directory.GetFiles(Application.dataPath + "/" + path);

			foreach (string fileName in fileEntries)
			{
				int assetPathIndex = fileName.IndexOf("Assets");
				string localPath = fileName.Substring(assetPathIndex);

				UnityEngine.Object t = UnityEditor.AssetDatabase.LoadAssetAtPath(localPath, typeof(T));

				if (t != null)
					al.Add(t);
			}
			T[] result = new T[al.Count];
			for (int i = 0; i < al.Count; i++)
				result[i] = (T)al[i];

			return result;
#else
      return null;
#endif
		}
		#endregion

		#region Resources
		/// <summary>
		/// Currently loaded resources
		/// </summary>
		private static Dictionary<Type, Dictionary<string, UnityEngine.Object>> loadedResources = new Dictionary<Type, Dictionary<string, UnityEngine.Object>>();

		/// <summary>
		/// Loads the resources of the given type, then returns the one with the given name
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="name"></param>
		/// <returns>The resource, if it has been found</returns>
		public static T GetResource<T>(string name) where T : UnityEngine.Object
		{
			Type type = typeof(T);
			if (!loadedResources.ContainsKey(type))
			{
				AddResourcesOfType<T>();
			}

			T resource = (T)loadedResources[type][name];
			return resource;
		}

		/// <summary>
		/// Loads the resources of the given type, returning them if they were found
		/// </summary>
		public static IEnumerable<T> GetResources<T>() where T : UnityEngine.Object
		{
			Type type = typeof(T);
			if (!loadedResources.ContainsKey(type))
			{
				AddResourcesOfType<T>();
			}
			var resources = loadedResources[type];
			return resources.Values.Select(x => (T)x);
		}

		/// <summary>
		/// Loads the resources of the given type, returning them if they were found
		/// </summary>
		public static Dictionary<string, T> GetResourceMap<T>() where T : UnityEngine.Object
		{
			Type type = typeof(T);
			if (!loadedResources.ContainsKey(type))
			{
				AddResourcesOfType<T>();
			}
			var resources = loadedResources[type];
			return resources.TransformValues(x => (T)x);
		}

		/// <summary>
		/// Loads all resources of the given type
		/// </summary>
		/// <typeparam name="T"></typeparam>
		private static void AddResourcesOfType<T>() where T : UnityEngine.Object
		{
			Type type = typeof(T);
			loadedResources.Add(type, new Dictionary<string, UnityEngine.Object>());
			T[] resources = Resources.FindObjectsOfTypeAll<T>();
			foreach (var resource in resources)
			{
				StratusDebug.Log($"Loaded {resource.name}");
				loadedResources[type].Add(resource.name, resource);
			}

		}

		private static void AddResourcesOfType(Type type)
		{
			loadedResources.Add(type, new Dictionary<string, UnityEngine.Object>());
			UnityEngine.Object[] resources = Resources.FindObjectsOfTypeAll(type);
			foreach (var resource in resources)
			{
				loadedResources[type].Add(resource.name, resource);
			}

		}
		#endregion

		#region Scriptable Objects
		/// <summary>
		/// Loads the scriptable object from an Unity relative path. 
		/// Returns null if the object doesn't exist.
		/// </summary>
		/// <typeparam name="T">The type of ScriptableObject</typeparam>
		/// <param name="path">The relative path to the asset. (e.g: "Assets/Resources/MyFile.asset")</param>
		/// <returns>The saved data as a ScriptableObject.</returns>
		public static T LoadScriptableObject<T>(string path) where T : ScriptableObject
		{
			var resourcesFolder = string.Concat(UnityDirectorySeparator, ResourcesFolderName, UnityDirectorySeparator);
			if (!path.Contains(resourcesFolder))
			{
				var exceptionMessage = string.Format(
				  "Failed to load ScriptableObject of type, {0}, from path: {1}. " +
				  "Path must begin with Assets and include a directory within the Resources folder.",
				  typeof(T).ToString(),
				  path);
				throw new UnityException(exceptionMessage);
			}

			// Mkae sure we are using a relative path
			var resourceRelativePath = FileUtility.MakeRelative(path);
			// Remove the file extension
			var fileExtension = Path.GetExtension(path);
			resourceRelativePath = resourceRelativePath.Replace(fileExtension, string.Empty);
			//Trace.Script("Loading data from " + resourceRelativePath);
			return Resources.Load<T>(resourceRelativePath);

		}

		/// <summary>
		/// Loads the saved data, stored as a ScriptableObject at the specified path. If
		/// the file or folders don't exist, it creates them.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="path"></param>
		/// <returns></returns>
		public static T LoadOrCreateScriptableObject<T>(string path) where T : ScriptableObject
		{
#if UNITY_EDITOR
			T data = null;

			// Try loading it at the specified path
			if (data == null)
			{
				data = UnityEditor.AssetDatabase.LoadAssetAtPath<T>(path);
			}

			// Try finding it anywhere
			if (data == null)
			{
				var objs = FindAndLoadAssetsByType<T>();
				if (objs.Length > 0)
					data = objs.First();
			}

			// Try creating it
			if (data == null)
			{
				data = CreateScriptableObject<T>(path);
			}

			return data;
#else
      return null;
#endif
		}

		public static T CreateScriptableObject<T>(string path) where T : ScriptableObject
		{
#if UNITY_EDITOR
			StratusDebug.Log(path + " has not been saved, creating it!");
			T data = ScriptableObject.CreateInstance<T>();
			CreateAssetAndDirectories(data, path);
			UnityEditor.AssetDatabase.SaveAssets();
			return data;
#else
      return null;
#endif
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
#if UNITY_EDITOR
			var instance = ScriptableObject.CreateInstance<T>();
			instance.hideFlags = HideFlags.HideInHierarchy;
			UnityEditor.AssetDatabase.AddObjectToAsset(instance, assetObject);
			UnityEditor.AssetDatabase.ImportAsset(UnityEditor.AssetDatabase.GetAssetPath(instance));
			UnityEditor.AssetDatabase.SaveAssets();
			UnityEditor.AssetDatabase.Refresh();
			return instance;
#else
      return null;
#endif
		}

		/// <summary>
		/// Adds an instance of the specified ScriptableObject class as a child
		/// to the parent ScriptableObject
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="assetObject"></param>
		/// <returns></returns>
		public static ScriptableObject AddInstanceToAsset(UnityEngine.Object assetObject, Type type)
		{
#if UNITY_EDITOR
			var instance = ScriptableObject.CreateInstance(type);
			instance.hideFlags = HideFlags.HideInHierarchy;
			UnityEditor.AssetDatabase.AddObjectToAsset(instance, assetObject);
			UnityEditor.AssetDatabase.ImportAsset(UnityEditor.AssetDatabase.GetAssetPath(instance));
			UnityEditor.AssetDatabase.SaveAssets();
			UnityEditor.AssetDatabase.Refresh();
			return instance;
#else
      return null;
#endif
		}

		/// <summary>
		/// Returns all assets of a specified type (loading them if necessary)
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public static T[] FindAndLoadAssetsByType<T>() where T : ScriptableObject
		{
#if UNITY_EDITOR
			List<T> assets = new List<T>();
			var typeName = typeof(T).ToString().Replace("UnityEngine.", "");
			string[] guids = UnityEditor.AssetDatabase.FindAssets(string.Format("t:{0}", typeName));
			for (int i = 0; i < guids.Length; ++i)
			{
				string assetPath = UnityEditor.AssetDatabase.GUIDToAssetPath(guids[i]);
				T asset = UnityEditor.AssetDatabase.LoadAssetAtPath<T>(assetPath);
				if (asset != null)
				{
					assets.Add(asset);
				}
			}
			return assets.ToArray();
#else
      return null;
#endif
		}
		#endregion

	}
}