using Newtonsoft.Json;

using Stratus.Editor;
using Stratus.Extensions;
using Stratus.Logging;
using Stratus.Unity.Editor.UIElements;
using Stratus.Unity.Extensions;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using UnityEditor;
using UnityEditor.UIElements;

using UnityEngine;
using UnityEngine.UIElements;

namespace Stratus.Unity.Editor.Packages
{
	[Serializable]
	public class PackageManifest
	{
		public class Author
		{
			public string name;
			public string email;
		}

		public string name;
		public string version;
		public string displayName;
		public string description;
		public string unity;
		public Dictionary<string, string> dependencies = new Dictionary<string, string>();
		public List<string> keywords = new ();
		public Author author = new Author();
	}

	public class PackageEditorWindow : StratusEditorWindow
	{
		private class Entry
		{
			public string assetPath { get; }
			public string displayName { get; }
			public Lazy<TextAsset> textAsset { get; }
			public string json => textAsset.Value.text;

			public PackageManifest manifest { get; private set; }
			public bool loaded => manifest != null;

			public Entry(string assetPath)
			{
				this.assetPath = assetPath;
				displayName = assetPath.Replace($"Assets/", string.Empty);
				textAsset = new Lazy<TextAsset>(() => AssetDatabase.LoadAssetAtPath<TextAsset>(assetPath));
			}

			public void Load()
			{
				manifest = JsonConvert.DeserializeObject<PackageManifest>(json);
			}

			public void Save()
			{
				var serialization = JsonConvert.SerializeObject(manifest);
				Save(serialization);
			}

			public void Save(string json)
			{
				var path = FileUtil.GetPhysicalPath(assetPath);
				File.WriteAllText(path, json);
				Debug.Log($"Updated manifest at {assetPath}:\n{json}");
				// TODO: Refresh correctly
				EditorUtility.SetDirty(textAsset.Value);
			}

			public override string ToString() => assetPath;
		}

		private ListView list;
		private VisualElement view;
		private List<Entry> packages { get; } = new List<Entry>();

		private Entry current => packages.Count > 0 ?
			packages[list.selectedIndex] : null;

		public const string packageFilename = "package.json";
		public const string _title = "Package Editor";

		[MenuItem(windowMenu + "Package Editor")]
		public static void Open()
		{
			OpenWindow<PackageEditorWindow>(_title);
		}

		private void CreateGUI()
		{
			VisualElement root = rootVisualElement;
			var asset = Resources.Load<VisualTreeAsset>(nameof(PackageEditorWindow));
			var tree = asset.Instantiate();
			root.Add(tree);

			var toolbar = root.Q<Toolbar>();
			var add = new ToolbarButton(Add);
			add.text = "Add";
			toolbar.Add(add);
			var refresh = new ToolbarButton(Refresh);
			refresh.text = "Refresh";
			toolbar.Add(refresh);

			var main = root.Q("Main");
			list = main.Q<ListView>("List");
			list.itemsSource = packages;
			list.selectionType = SelectionType.Single;
			list.makeItem = MakeItem;
			list.bindItem = BindItem;
			list.selectionChanged += OnSelectionChanged;
			view = main.Q("View");
			view.Add(new JsonInspector());

			Refresh();
		}

		private void OnSelectionChanged(IEnumerable<object> items)
		{
			var entry = items.FirstOrDefault() as Entry;
			Display(entry);
		}

		private void Display(Entry entry)
		{
			var inspector = view.Q<JsonInspector>();
			entry.Load();

			if (!entry.loaded)
			{
				return;
			}

			try
			{
				inspector.Set(entry.json, entry.Save);
			}
			catch (Exception e)
			{
				Debug.LogError(e);
			}
		}

		public void Save()
		{
			if (current != null && current.loaded)
			{
				current.Save();
			}
		}

		private void Add()
		{
			var path = EditorUtility.SaveFilePanel("Add Package Manifest",
				"Assets", packageFilename, "json");
			if (path != null)
			{
				var manifest = new PackageManifest();
				var json = JsonConvert.SerializeObject(manifest, Formatting.Indented);
				File.WriteAllText(path, json);
				Debug.Log($"Added new package manifest at {path}");
				Refresh();
			}
		}

		private void Refresh()
		{
			SearchForPackageManifestsInProject();
		}

		private void BindItem(VisualElement visualElement, int index)
		{
			var entry = packages[index];
			var label = visualElement as Label;

			string text = entry.displayName;

			label.text = text;
		}

		private VisualElement MakeItem()
		{
			var label = new Label();
			label.enableRichText = true;
			label.style.unityTextAlign = TextAnchor.MiddleLeft;
			label.RegisterCallback<ClickEvent>(e =>
			{
				if (e.clickCount > 1)
				{
					var entry = packages[list.selectedIndex];
					var asset = entry.textAsset.Value;
					Selection.activeObject = asset;
				}
			});
			return label;
		}

		private void SearchForPackageManifestsInProject()
		{
			packages.Clear();

			var assetPaths = Utility.AssetUtility.FindAssetPaths<TextAsset>("package.json");
			if (assetPaths.IsNullOrEmpty())
			{
				this.LogWarning("No package manfiests were found");
				return;
			}

			packages.AddRange(assetPaths.Select(p => new Entry(p)));
			list.RefreshItems();
			//var _packages = assetPaths.Transform(x => UnityEditor.PackageManager.PackageInfo.FindForAssetPath(x));
			this.Log($"Found {packages.Count} packages ({assetPaths.ToStringJoin(Environment.NewLine)})");
		}
	}

}