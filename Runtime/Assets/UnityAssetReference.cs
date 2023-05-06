using UnityEngine;
using System;
using Stratus.Extensions;
using Stratus.Unity;

namespace Stratus.Unity
{
	[Serializable]
	public abstract class UnityAssetReference<TAsset> : StratusAsset
		where TAsset : UnityEngine.Object
	{
		[SerializeField]
		private TAsset[] _references;
		public TAsset reference => GetAsset(_references);
		protected virtual TAsset GetAsset(TAsset[] values) => values.Random();

		public UnityAssetReference()
		{
		}

		public UnityAssetReference(string name, params TAsset[] assets)
			: base(name)
		{
			_references = assets;
		}
	}

	[Serializable]
	public abstract class UnityAssetReference<TAsset, TParameter> : UnityAssetReference<TAsset>
		where TAsset : UnityEngine.Object
		where TParameter : class, new()
	{
		[SerializeField]
		private TParameter _parameters;
		public TParameter parameters => _parameters;
		public bool hasParameters => _parameters != null;

		protected UnityAssetReference()
		{
		}

		protected UnityAssetReference(string name, params TAsset[] assets) : base(name, assets)
		{
		}
	}
}