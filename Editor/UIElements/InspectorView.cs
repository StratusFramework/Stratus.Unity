using Newtonsoft.Json.Linq;

using Stratus.Extensions;
using Stratus.Reflection;
using Stratus.Types;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEditor;
using UnityEditor.UIElements;

using UnityEngine.UIElements;

namespace Stratus.Unity.Editor.UIElements
{
	public class InspectorView : VisualElement
	{
		public enum Mode
		{
			/// <summary>
			/// Inspecting a <see cref="Type"/>
			/// </summary>
			Default,
			/// <summary>
			/// Inspecting tokenized JSON
			/// </summary>
			Tokenized
		}

		public class TokenizedObject
		{
			internal string original;
			internal Dictionary<JObject, VisualElement> hierarchy = new Dictionary<JObject, VisualElement>();
		}

		private VisualElement view;
		private ToolbarButton saveButton;
		private TokenizedObject tokenizedObject;

		private Action save;

		public Mode mode { get; private set; }

		public InspectorView()
		{
			var toolbar = new Toolbar();
			saveButton = new ToolbarButton(Save);
			saveButton.text = "Save";
			toolbar.Add(saveButton);
			Add(toolbar);

			view = new ScrollView();
			Add(view);
		}

		public void Save()
		{
			save?.Invoke();
		}

		#region Class
		public void Set(object target, Action onSave)
		{
			view.Clear();
			mode = Mode.Default;

			var inspector = new Inspector(target);
			foreach (var node in inspector.nodes)
			{
				Add(node, view);
			}

			save = onSave;
		}

		private void Add(Node node, VisualElement parent)
		{
			var visualElement = Get(node);
			if (visualElement != null)
			{
				parent.Add(visualElement);
				foreach (var child in node.children.Values)
				{
					Add(child, visualElement);
				}
			}
		}

		private VisualElement Get(Node node)
		{
			if (node.type == typeof(string))
			{
				var textField = new TextField();
				textField.label = DisplayName(node.name);
				textField.value = node.Get<string>();
				textField.RegisterValueChangedCallback(e =>
				{
					node.value = e.newValue;
				});

				return textField;
			}
			else if (node.type == typeof(int))
			{
				var intField = new IntegerField(name);
				intField.value = node.Get<int>();
				intField.RegisterValueChangedCallback(e =>
				{
					node.value = e.newValue;
				});
				return intField;
			}
			else if (node.isCollection)
			{
				var foldout = new Foldout();
				foldout.text = DisplayName(node.name);
				switch (node.collectionType)
				{
					case CollectionType.Unsupported:
						foldout.Add(new Label($"Unsupported collection type {node.type}"));
						break;

					case CollectionType.List:
						{
							var values = node.Get<IList>();
							var listView = new ListView();
							listView.showAddRemoveFooter = true;
							listView.itemsSource = values;
							listView.makeItem = () => new TextField(string.Empty);
							listView.bindItem = (ve, i) =>
							{
								var textField = ve as TextField;
								textField.label = null;
								textField.value = (string)values[i];
								textField.RegisterValueChangedCallback(e =>
								{
									values[i] = e.newValue;
								});
							};
							foldout.Add(listView);
						}
						break;

					case CollectionType.Dictionary:
						{
							var dictView = new DictionaryView();
							var dictionary = node.Get<IDictionary>();
							dictView.itemsSource = dictionary;
							dictView.showAddRemoveFooter = true;
							foldout.Add(dictView);
						}
						break;
				}
				return foldout;
			}
			else if (node.hasChildren)
			{
				var foldout = new Foldout();
				foldout.text = DisplayName(node.name);
				return foldout;
			}

			return null;
		}
		#endregion

		#region Tokenized
		public void Set(string json, Action<string> onSave)
		{
			view.Clear();
			mode = Mode.Tokenized;

			tokenizedObject = new TokenizedObject();
			tokenizedObject.original = json;

			var jo = JObject.Parse(json);
			tokenizedObject.hierarchy.Add(jo, view);
			Add(jo);

			save = () =>
			{
				var serialization = jo.ToString();
				onSave(serialization);
			};
		}

		private void Add(JObject jo, string name = null, JObject parent = null)
		{
			VisualElement parentView;

			if (name != null)
			{
				var foldout = new Foldout();
				foldout.text = DisplayName(name);
				parentView = foldout;

				tokenizedObject.hierarchy.Add(jo, foldout);
				var parentElement = tokenizedObject.hierarchy[parent];
				parentElement.Add(foldout);
			}
			else
			{
				parentView = view;
			}

			foreach (var child in jo)
			{
				var visualElement = Get(child.Key, child.Value.Type, jo);
				if (visualElement != null)
				{
					parentView.Add(visualElement);
				}
			}
		}

		private VisualElement Get(string name, JTokenType type, JObject parent)
		{
			switch (type)
			{
				case JTokenType.Object:
					var jo = parent[name] as JObject;
					Add(jo, name, parent);
					break;

				case JTokenType.Array:
					{
						var foldout = new Foldout();
						foldout.text = DisplayName(name);
						var listView = new ListView();
						foldout.Add(listView);

						var container = parent[name] as JContainer;
						var tokens = container.Values();
						List<string> values = new List<string>();
						values.AddRange(tokens.Select(t => t.Value<string>()));

						listView.itemsSource = values;
						listView.makeItem = () => new TextField(string.Empty);
						listView.bindItem = (ve, i) =>
						{
							var textField = ve as TextField;
							textField.label = null;
							textField.value = values[i];
							textField.RegisterValueChangedCallback(e =>
							{
								values[i] = e.newValue;
								container.ElementAt(i).Replace(e.newValue);
							});
						};
						listView.showAddRemoveFooter = true;
						listView.itemsAdded += indeces =>
						{
							var replacement = values.Cast<object>().ToArray();
							container.ReplaceAll(replacement);
						};
						listView.itemsRemoved += indeces =>
						{
							var replacement = values.Cast<object>().ToArray();
							container.ReplaceAll(replacement);
						};

						return foldout;
					}

				case JTokenType.Integer:
					var intField = new IntegerField(name);
					intField.value = parent[name].Value<int>();
					intField.RegisterValueChangedCallback(e =>
					{
						parent[name].Replace(e.newValue);
					});
					return intField;

				case JTokenType.Float:
					var floatField = new FloatField(name);
					floatField.value = parent[name].Value<float>();
					floatField.RegisterValueChangedCallback(e =>
					{
						parent[name].Replace(e.newValue);
					});
					return floatField;

				case JTokenType.String:
					{
						var textField = new TextField();
						textField.label = DisplayName(name);
						textField.value = parent[name].Value<string>();
						textField.RegisterValueChangedCallback(e =>
						{
							parent[name].Replace(e.newValue);
						});

						return textField;
					}
				case JTokenType.Boolean:
					var toggle = new Toggle(name);
					toggle.value = parent[name].Value<bool>();
					toggle.RegisterValueChangedCallback(e =>
					{
						parent[name].Replace(e.newValue);
					});
					return toggle;
				case JTokenType.Null:
					break;
				case JTokenType.Undefined:
					break;
				case JTokenType.Date:
					break;
				case JTokenType.Raw:
					break;
				case JTokenType.Bytes:
					break;
				case JTokenType.Guid:
					break;
				case JTokenType.Uri:
					break;
				case JTokenType.TimeSpan:
					break;
				default:
					break;
			}

			return null;
		}
		#endregion



		private static string DisplayName(string name)
			 => ObjectNames.NicifyVariableName(name);
	}
}
