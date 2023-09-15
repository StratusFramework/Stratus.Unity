using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UIElements;
using System;
using Stratus.Types;
using Unity.VisualScripting.YamlDotNet.Core.Tokens;
using UnityEngine.UI;

namespace Stratus.Unity.Editor.UIElements
{
	/// <summary>
	/// Used for inspecting a <see cref="IDictionary"/>.
	/// The <see cref="itemsSource"/> must be set.
	/// </summary>
	public class DictionaryView : VisualElement
	{
		public enum KeyType
		{
			String,
			Integer
		}

		#region Declarations
		private class Entry
		{
			public object key;
			public object value;
		}

		private class Instance
		{
			internal List<Entry> list;
			internal Type itemsSourceType;
			internal Type keyType;
			internal Type valueType;

			internal Instance(IDictionary dictionary)
			{
				itemsSourceType = dictionary.GetType();
				var types = TypeUtility.GetKeyValueType(itemsSourceType);
				keyType = types.keyType;
				valueType = types.valueType;
				list = FromDictionaryToList(dictionary);
			}
		}

		private const string keyLabel = "Key";
		private const string valueLabel = "Value";
		#endregion

		#region Fields
		private ListView view;
		private Instance instance;
		#endregion

		#region Properties
		public IDictionary itemsSource
		{
			get => _itemsSource;
			set
			{
				_itemsSource = value;
				instance = new Instance(value);
				view.itemsSource = instance.list;
				view.RefreshItems();
			}
		}

		private IDictionary _itemsSource;

		public bool showAddRemoveFooter
		{
			get => view.showAddRemoveFooter;
			set => view.showAddRemoveFooter = value;
		}
		#endregion

		public DictionaryView()
		{
			Reset();
		}

		public void Reset()
		{
			instance = null;

			if (view != null)
			{
				Remove(view);
			}

			view = new ListView();
			view.makeItem = MakeItem;
			view.bindItem = BindItem;
			view.itemsAdded += this.OnItemsAdded;
			view.itemsRemoved += this.OnItemsRemoved;

			Add(view);
		}

		private VisualElement MakeItem()
		{
			var root = new VisualElement();
			root.name = "Entry";
			root.style.flexDirection = FlexDirection.Row;
			root.style.alignItems = Align.Center;
			root.style.paddingLeft = root.style.paddingRight = 4;

			void add(Type type, string name)
			{
				Label label = new Label(name);
				root.Add(label);

				VisualElement ve = null;
				if (type == typeof(int))
				{
					ve = new IntegerField();
				}
				else if (type == typeof(string))
				{
					ve = new TextField();
				}
				else
				{
					ve = new Label($"Unsupported <{instance.keyType.Name}>");
				}

				ve.style.flexShrink = 1f;
				ve.name = name;
				root.Add(ve);
			}

			add(instance.keyType, keyLabel);
			add(instance.valueType, valueLabel);

			return root;
		}

		private void BindItem(VisualElement ve, int index)
		{
			var entry = instance.list[index];

			// KEY
			if (instance.keyType == typeof(int))
			{
				var field = ve.Q<IntegerField>(keyLabel);
				field.value = (int)entry.key;
			}
			else if (instance.keyType == typeof(string))
			{
				var field = ve.Q<TextField>(keyLabel);
				field.value = (string)entry.key;
			}

			// VALUE
			if (instance.valueType == typeof(int))
			{
				var field = ve.Q<IntegerField>(valueLabel);
				field.value = (int)entry.value;
			}
			else if (instance.valueType == typeof(string))
			{
				var field = ve.Q<TextField>(valueLabel);
				field.value = (string)entry.value;
			}
		}

		private void OnItemsRemoved(IEnumerable<int> obj)
		{
		}

		private void OnItemsAdded(IEnumerable<int> obj)
		{
		}

		private static List<Entry> FromDictionaryToList(IDictionary dictionary)
		{
			var list = new List<Entry>();
			foreach (DictionaryEntry kvp in dictionary)
			{
				var entry = new Entry()
				{
					key = kvp.Key,
					value = kvp.Value
				};
				list.Add(entry);
			}
			return list;
		}
	}
}
