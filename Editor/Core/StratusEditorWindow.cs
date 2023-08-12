﻿using Stratus.Logging;
using Stratus.Unity.Editor;

using System;
using System.Collections.Generic;

using UnityEditor;
using UnityEditor.AnimatedValues;

using UnityEngine;

namespace Stratus.Editor
{
	/// <summary>
	/// Base class for editor windows used by the Stratus Framework
	/// </summary>
	public abstract class StratusEditorWindow : EditorWindow, IStratusLogger
	{
		protected const string rootMenu = "Stratus/";
		protected const string windowMenu = rootMenu + "Windows/";

		protected SerializedObject serializedObject { get; set; }
		protected StratusSerializedPropertyMap serializedPropertyMap { get; set; }

		/// <summary>
		/// Opens the editor window, such as from a <see cref="MenuItem"/>
		/// </summary>
		/// <param name="title"></param>
		/// <param name="utility"></param>
		protected static void OpenWindow<T>(string title = null, bool utility = false)
		{
			Type type = typeof(T);
			title = title != null ? title : type.Name;
			var wnd = GetWindow(type, utility, title);
		}
	}

	/// <summary>
	/// Base (singleton) class for editor windows used by the Stratus Framework
	/// </summary>
	public abstract class StratusEditorWindow<T> : StratusEditorWindow where T : EditorWindow
	{
		#region Properties
		/// <summary>
		/// The active instance for this editor window
		/// </summary>
		protected static T instance { get; private set; }

		/// <summary>
		/// A drawer for an optional menu bar
		/// </summary>
		protected MenuBarGUIObject menuBarDrawer { get; set; }

		/// <summary>
		/// A map of all objects currently being inspected
		/// </summary>
		protected Dictionary<UnityEngine.Object, StratusSerializedPropertyMap> inspectedObjects { get; private set; }

		/// <summary>
		/// A map of all objects currently being inspected
		/// </summary>
		protected Dictionary<UnityEngine.Object, UnityEditor.Editor> objectEditors { get; private set; }

		/// <summary>
		/// The rect used by this window in GUI space, where top left is at position(0,0)
		/// </summary>
		protected Rect positionToGUI => GUIUtility.ScreenToGUIRect(this.position);

		/// <summary>
		/// Computes the current avaialble position within the window, after taking into account
		/// the height consumed by the latest control
		/// </summary>
		protected Rect currentPosition
		{
			get
			{

				Rect lastRect = GUILayoutUtility.GetLastRect();
				Rect available = this.positionToGUI;
				available.x += lastRect.x;
				available.y += lastRect.height + lastRect.y;
				available.height -= lastRect.y;
				return available;
			}
		}

		public static float padding => StratusEditorGUI.standardPadding;
		public static float lineHeight => StratusEditorUtility.lineHeight; 
		#endregion

		#region Virtual
		protected abstract void OnWindowEnable();
		protected abstract void OnWindowGUI();
		protected virtual MenuBarGUIObject OnSetMenuBar() => null; 
		#endregion

		#region Messages
		private void OnEnable()
		{
			instance = this as T;

			this.serializedObject = new SerializedObject(this);
			this.serializedPropertyMap = new StratusSerializedPropertyMap(this.serializedObject);
			this.inspectedObjects = new Dictionary<UnityEngine.Object, StratusSerializedPropertyMap>
			{
				{ this, this.serializedPropertyMap }
			};
			this.objectEditors = new Dictionary<UnityEngine.Object, UnityEditor.Editor>();

			this.menuBarDrawer = this.OnSetMenuBar();
			this.OnWindowEnable();
			EditorApplication.playModeStateChanged += this.OnPlayModeStateChange;
		}

		private void OnGUI()
		{
			EditorGUILayout.Space();
			this.menuBarDrawer?.Draw(this.currentPosition);
			this.OnWindowGUI();
		}

		private void Update()
		{
			this.OnWindowUpdate();
		}

		protected virtual void OnPlayModeStateChange(PlayModeStateChange stateChange)
		{
		}

		protected virtual void OnWindowUpdate()
		{
		}
		#endregion

		#region Static
		/// <summary>
		/// Opens the editor window, such as from a <see cref="MenuItem"/>
		/// </summary>
		/// <param name="title"></param>
		/// <param name="utility"></param>
		protected static void OpenWindow(string title = null, bool utility = false)
		{
			Type type = typeof(T);
			title = title != null ? title : type.Name;
			EditorWindow.GetWindow(type, utility, title);
		}
		#endregion

		#region GUI
		protected void InspectProperties(string label = "Properties")
		{
			this.InspectProperties(label, this.serializedPropertyMap.properties);
		}

		protected void InspectProperty(string propertyName)
		{
			SerializedProperty property = this.serializedPropertyMap.GetProperty(propertyName);
			EditorGUILayout.PropertyField(property);
		}

		protected void InspectProperty(string propertyName, string label)
		{
			SerializedProperty property = this.serializedPropertyMap.GetProperty(propertyName);
			EditorGUILayout.PropertyField(property, new GUIContent(label));
		}

		protected bool InspectProperty(SerializedProperty serializedProperty, string label)
		{
			bool changed = StratusEditorGUILayout.Property(serializedProperty, label);
			foreach (SerializedProperty child in serializedProperty.GetVisibleChildren())
			{
				changed |= this.InspectProperty(child);
			}
			return changed;
		}

		protected bool InspectProperty(SerializedProperty serializedProperty)
		{
			bool changed = StratusEditorGUILayout.Property(serializedProperty);
			foreach (SerializedProperty child in serializedProperty.GetVisibleChildren())
			{
				changed |= this.InspectProperty(child);
			}
			return changed;
		}

		protected void InspectProperties(string label, params SerializedProperty[] serializedProperties)
		{
			EditorGUILayout.LabelField(label, EditorStyles.centeredGreyMiniLabel);
			foreach (SerializedProperty property in serializedProperties)
			{
				this.InspectProperty(property, property.displayName);
			}
			EditorGUILayout.Separator();
		}

		protected void InspectProperties(UnityEngine.Object target, string label)
		{
			if (!this.inspectedObjects.ContainsKey(target))
			{
				this.inspectedObjects.Add(target, new StratusSerializedPropertyMap(target));
			}

			if (!this.objectEditors.ContainsKey(target))
			{
				var editor = UnityEditor.Editor.CreateEditor(target);
				if (editor is StratusEditor)
				{
					((StratusEditor)editor).drawTypeLabels = false;
				}
				this.objectEditors.Add(target, editor);

			}

			this.objectEditors[target].OnInspectorGUI();
		}

		protected bool InspectObjectFieldWithHeader<T>(ref T objectField, string label) where T : UnityEngine.Object
		{
			bool changed = StratusEditorGUILayout.ObjectFieldWithHeader(ref objectField, label);
			if (changed)
			{
				EditorUtility.SetDirty(this);
			}
			return changed;
		}

		protected AnimBool[] GenerateAnimBools(int count, bool value)
		{
			List<AnimBool> bools = new List<AnimBool>();
			for (int i = 0; i < count; ++i)
			{
				AnimBool animBool = new AnimBool(value);
				animBool.valueChanged.AddListener(this.Repaint);
				bools.Add(animBool);
			}
			return bools.ToArray();
		} 
		#endregion
	}
}