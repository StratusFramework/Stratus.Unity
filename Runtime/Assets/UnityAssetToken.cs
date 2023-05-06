using System;

namespace Stratus.Unity
{
	public abstract class UnityAssetToken<T>
		: StratusAssetToken<T>
		where T : UnityEngine.Object
	{
		protected UnityAssetToken(string name, Func<T> assetFunction) 
			: base(name, assetFunction)
		{
		}

		protected UnityAssetToken(string name, Func<string, T> aliasToAssetFunction) 
			: base(name, aliasToAssetFunction)
		{
		}

		protected override bool IsNull(T asset)
		{
			return Stratus.OdinSerializer.Utilities.UnityExtensions.SafeIsUnityNull(asset);
		}
	}

}