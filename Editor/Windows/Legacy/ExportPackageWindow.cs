using Stratus.Editor;
using Stratus.Unity.Extensions;
using Stratus.Unity.Utility;

using UnityEditor;

using UnityEngine;

namespace Stratus.Unity.Editor
{
	public class ExportPackageWindow : StratusEditorWindow<ExportPackageWindow>
	{
		//------------------------------------------------------------------------/
		// Fields
		//------------------------------------------------------------------------/
		[SerializeField]
		public ExportPackagePreset preset;
		private const string recentPresetKey = "Recent Export Package Preset";

		//------------------------------------------------------------------------/
		// Properties
		//------------------------------------------------------------------------/
		public bool hasPreset => !this.preset.IsNull();

		//------------------------------------------------------------------------/
		// Messages
		//------------------------------------------------------------------------/
		protected override void OnWindowEnable()
		{
			preset = StratusPreferences.GetObjectReference<ExportPackagePreset>(recentPresetKey);
		}

		protected override void OnWindowGUI()
		{
			if (this.InspectObjectFieldWithHeader(ref this.preset, "Preset"))
			{
				StratusPreferences.SaveObjectReference(recentPresetKey, preset);
			}
			if (this.hasPreset)
			{
				this.InspectProperties(this.preset, "Properties");
			}
			this.ExportControls();
		}

		[MenuItem(Constants.rootMenu + "Export Package")]
		public static void Open()
		{
			OpenWindow("Export Package");
		}

		//------------------------------------------------------------------------/
		// Procedures
		//------------------------------------------------------------------------/
		private static void Export(ExportPackageArguments arguments)
		{
			string location = FileUtility.GetFolderPath(arguments.path);
			AssetDatabase.ExportPackage(location, $"{arguments.name}.unitypackage", arguments.options);
			EditorUtility.RevealInFinder(location);
			StratusDebug.Log($"Exported {arguments.name} to {location}");
		}

		private void ExportControls()
		{
			StratusEditorGUILayout.BeginAligned(TextAlignment.Center);
			{
				StratusEditorGUILayout.Button("Export", () =>
				{

					if (this.hasPreset && this.preset.arguments.valid)
					{
						Export(this.preset.arguments);
					}

				});
			}
			StratusEditorGUILayout.EndAligned();
		}


	}

}