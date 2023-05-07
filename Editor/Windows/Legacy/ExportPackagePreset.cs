using Stratus.Extensions;
using Stratus.Unity.Extensions;
using Stratus.Unity.Data;

using System;

using UnityEditor;

using UnityEngine;

namespace Stratus.Unity.Editor
{
	[Serializable]
	public class ExportPackageArguments
	{
		[SerializeField]
		public string name;
		[SerializeField]
		public UnityEngine.Object asset;
		[SerializeField, EnumFlag]
		public ExportPackageOptions options = ExportPackageOptions.Default;
		[SerializeField]
		public string exportPath;

		public string path => AssetDatabase.GetAssetPath(asset);
		public bool valid => name.IsValid() && asset.IsValid();
	}

	[CreateAssetMenu(fileName = "Stratus Export Package Preset", menuName = "Stratus/Core/Export Package Preset")]
	public class ExportPackagePreset : StratusScriptable
	{
		[SerializeField]
		public ExportPackageArguments arguments;
	}

}