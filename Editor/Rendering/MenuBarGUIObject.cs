﻿using Stratus.Extensions;
using Stratus.Models;
using Stratus.Unity.Rendering;

using System.Collections.Generic;

using UnityEditor;

using UnityEngine;

namespace Stratus.Unity.Editor
{
	public class MenuBarGUIObject
	{
		//------------------------------------------------------------------------/
		// Declarations
		//------------------------------------------------------------------------/  
		private struct MenuOption
		{
			public GUIContent content;
			public GenericMenu.MenuFunction menuFunction;

			public MenuOption(GUIContent content, GenericMenu.MenuFunction menuFunction)
			{
				this.content = content;
				this.menuFunction = menuFunction;
			}

			public MenuOption(string content, GenericMenu.MenuFunction menuFunction)
			{
				this.content = new GUIContent(content);
				this.menuFunction = menuFunction;
			}
		}

		private class MenuColumn
		{
			public GUIContent content;
			public string name;
			public List<MenuOption> items = new List<MenuOption>();

			public MenuColumn(string name)
			{
				this.content = new GUIContent(name);
				this.name = name;
			}
		}

		//------------------------------------------------------------------------/
		// Fields
		//------------------------------------------------------------------------/  
		private List<MenuColumn> menu = new List<MenuColumn>();
		private Dictionary<string, MenuColumn> columns = new Dictionary<string, MenuColumn>();
		private List<MenuOption> addItems = new List<MenuOption>();
		private List<MenuOption> optionItems = new List<MenuOption>();
		private List<MenuOption> validateItems = new List<MenuOption>();
		public float menuOffset;

		//------------------------------------------------------------------------/
		// Properties
		//------------------------------------------------------------------------/  
		public StratusOrientation orientation { get; private set; }
		public bool hasMenu { get; private set; }
		public bool hasToolbar { get; private set; }
		public GUILayoutOption[] guiLayoutOptions { get; private set; }

		//------------------------------------------------------------------------/
		// CTOR
		//------------------------------------------------------------------------/  
		public MenuBarGUIObject(StratusOrientation orientation, params GUILayoutOption[] options)
		{
			this.orientation = orientation;
			switch (this.orientation)
			{
				case StratusOrientation.Horizontal:
					this.guiLayoutOptions = options;
					break;
				case StratusOrientation.Vertical:
					this.guiLayoutOptions = options;
					break;
				default:
					break;
			}
		}

		//------------------------------------------------------------------------/
		// Methods: Add
		//------------------------------------------------------------------------/
		public void AddItem(string content, GenericMenu.MenuFunction menuFunction, string columnName)
		{
			MenuColumn column = null;
			bool exists = this.columns.ContainsKey(columnName);
			if (!exists)
			{
				column = new MenuColumn(columnName);
				this.columns.Add(columnName, column);
				this.menu.Add(column);
			}
			else
			{
				column = this.columns[columnName];
			}

			column.items.Add(new MenuOption(content, menuFunction));

			this.hasMenu = true;
		}

		public void AddItem(GUIContent content, GenericMenu.MenuFunction menuFunction, StratusEditorGUILayout.ContextMenuType type)
		{
			MenuOption item = new MenuOption(content, menuFunction);
			switch (type)
			{
				case StratusEditorGUILayout.ContextMenuType.Add:
					addItems.Add(item);
					break;
				case StratusEditorGUILayout.ContextMenuType.Validation:
					validateItems.Add(item);
					break;
				case StratusEditorGUILayout.ContextMenuType.Options:
					optionItems.Add(item);
					break;
			}

			this.hasToolbar = true;
		}

		public void AddItem(string content, GenericMenu.MenuFunction menuFunction, StratusEditorGUILayout.ContextMenuType type)
		{
			this.AddItem(new GUIContent(content), menuFunction, type);
		}

		//------------------------------------------------------------------------/
		// Methods: Draw
		//------------------------------------------------------------------------/
		public void Draw(Rect rect)
		{
			switch (this.orientation)
			{
				case StratusOrientation.Horizontal:
					if (this.hasMenu)
					{
						EditorGUILayout.BeginHorizontal();
						this.DrawMenu(rect);
						EditorGUILayout.EndHorizontal();
					}
					if (this.hasToolbar)
					{
						EditorGUILayout.BeginHorizontal();
						this.DrawToolbar(rect);
						EditorGUILayout.EndHorizontal();
					}
					break;

				case StratusOrientation.Vertical:
					if (this.hasMenu)
					{
						EditorGUILayout.BeginVertical();
						this.DrawMenu(rect);
						EditorGUILayout.EndVertical();
					}
					if (this.hasToolbar)
					{
						EditorGUILayout.BeginVertical();
						this.DrawToolbar(rect);
						EditorGUILayout.EndVertical();
					}
					break;
			}
		}

		private void DrawMenu(Rect rect)
		{
			GUIStyle style = Styles.button;
			Vector2 offset = Vector2.zero;

			// Set dynamically
			rect.size = Vector2.zero;

			foreach (var column in this.menu)
			{
				Vector2 size = style.CalcSize(column.content);
				offset.y = size.y;

				if (GUILayout.Button(column.name, style, this.guiLayoutOptions))
				{
					GenericMenu menu = new GenericMenu();
					foreach (var item in column.items)
					{
						menu.AddItem(item.content, false, item.menuFunction);
					}

					rect.position += offset;
					menu.DropDown(rect);
				}

				offset.x += size.x;
			}
		}

		private void DrawToolbar(Rect rect)
		{
			this.DrawContextMenu(StratusEditorGUILayout.ContextMenuType.Add, this.addItems);
			this.DrawContextMenu(StratusEditorGUILayout.ContextMenuType.Options, this.optionItems);
			this.DrawContextMenu(StratusEditorGUILayout.ContextMenuType.Validation, this.validateItems);
		}

		private void DrawContextMenu(StratusEditorGUILayout.ContextMenuType type, List<MenuOption> options)
		{
			if (options.IsValid())
			{
				var menu = new GenericMenu();
				foreach (var option in options)
				{
					menu.AddItem(option.content, false, option.menuFunction);
				}
				StratusEditorGUILayout.ContextMenu(menu, type);
			}
		}
	}
}