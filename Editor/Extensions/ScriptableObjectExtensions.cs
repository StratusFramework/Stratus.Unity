using Stratus.Unity.Utility;

using System;

using UnityEngine;

namespace Stratus.Unity.Editor
{
	public static class ScriptableObjectExtensions
	{
		public static T AddInstanceToAsset<T>(this ScriptableObject assetObject) where T : ScriptableObject
		{
			return AssetUtility.AddInstanceToAsset<T>(assetObject);
		}

		public static ScriptableObject AddInstanceToAsset(this ScriptableObject assetObject, Type type)
		{
			return Stratus.Unity.Utility.AssetUtility.AddInstanceToAsset(assetObject, type);
		}
	}

}