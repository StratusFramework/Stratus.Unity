using System;

namespace Stratus.Unity
{
	public class UnityAssetQuery<AssetType> : StratusAssetQuery<AssetType>
		where AssetType : UnityEngine.Object
	{
		public UnityAssetQuery(Func<AssetType[]> getAssetsFunction) : base(getAssetsFunction, x => x.name)
		{
		}
	}

}