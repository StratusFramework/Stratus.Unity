﻿using Stratus.Extensions;
using Stratus.Reflection;

using System;
using System.Collections.Generic;
using System.Reflection;

using UnityEditor;

using UnityEngine;

namespace Stratus.Unity.Editor
{
	public partial class StratusSerializedEditorObject
	{
		/// <summary>
		/// Draws all the fields in a System.Object
		/// </summary>
		public class DefaultObjectDrawer : ObjectDrawer
		{
			//------------------------------------------------------------------------/
			// Properties
			//------------------------------------------------------------------------/
			public DefaultObjectDrawer parent { get; private set; }
			public DrawCommand[] drawCalls { get; private set; }
			//public bool[] isField { get; private set; }

			public bool isArray { get; private set; }
			public bool isField { get; private set; }

			//------------------------------------------------------------------------/
			// CTOR
			//------------------------------------------------------------------------/
			public DefaultObjectDrawer(Type type) : base(type)
			{
				this.height = lineHeight;
				this.drawCalls = this.GenerateDrawCommands(this.fields);
				this.isDrawable = this.drawCalls.IsValid();
			}

			public DefaultObjectDrawer(FieldInfo field, Type type) : base(type)
			{
				this.displayName = ObjectNames.NicifyVariableName(field.Name);
				this.height = lineHeight;
				this.drawCalls = this.GenerateDrawCommands(this.fields);
				this.isDrawable = this.drawCalls.IsValid();
			}

			//------------------------------------------------------------------------/
			// Methods: Draw
			//------------------------------------------------------------------------/
			public override bool DrawEditorGUILayout(object target, bool isChild = false)
			{
				bool changed = false;
				string content = this.isDrawable ? this.displayName : $"No serialized fields available";
				UnityEditor.EditorGUILayout.LabelField(content);

				if (isChild)
				{
					EditorGUI.indentLevel++;
				}

				foreach (DrawCommand call in this.drawCalls)
				{
					if (call.isField)
					{
						changed |= call.drawer.DrawEditorGUILayout(target);
					}
					else
					{
						object value = this.GetValueOrSetDefault(call.field, target);
						changed |= call.drawer.DrawEditorGUILayout(value, true);
					}

				}

				if (isChild)
				{
					EditorGUI.indentLevel--;
				}

				return changed;
			}

			public override bool DrawEditorGUI(Rect position, object target)
			{
				bool changed = false;
				string content = this.isDrawable ? this.displayName : $"No serialized fields to draw for {this.type.Name}";
				EditorGUI.LabelField(position, content);

				// Draw all drawers
				foreach (DrawCommand call in this.drawCalls)
				{
					if (call.isField)
					{
						changed |= call.drawer.DrawEditorGUI(position, target);
					}
					else
					{
						//UnityEditor.EditorGUILayout.BeginVertical(EditorStyles.helpBox);
						EditorGUI.indentLevel++;
						//position.y += lineHeight;
						object value = this.GetValueOrSetDefault(call.field, target);
						changed |= call.drawer.DrawEditorGUI(position, value);
						EditorGUI.indentLevel--;
						//UnityEditor.EditorGUILayout.EndVertical();
					}
					position.y += lineHeight;
				}
				return changed;
			}

			private object GetValueOrSetDefault(FieldInfo field, object target)
			{
				// Try to get the value from the taret
				object value;
				value = field.GetValue(target);
				// If the field hasn't been instantiated
				if (value == null)
				{
					value = Activator.CreateInstance(field.FieldType);
					field.SetValue(target, value);
				}
				return value;
			}



			protected DrawCommand[] GenerateDrawCommands(FieldInfo[] fields)
			{
				List<DrawCommand> drawers = new List<DrawCommand>();
				for (int i = 0; i < fields.Length; ++i)
				{
					FieldInfo field = fields[i];
					Type fieldType = field.FieldType;
					StratusSerializedFieldType serializedPropertyType = SerializedFieldTypeExtensions.Deduce(field);

					// Unity is supported by Unity if it's not a generic array
					bool isArray = IsArray(fieldType);
					bool isUnitySupportedType = (serializedPropertyType != StratusSerializedFieldType.Generic || isArray); //  OdinSerializer.FormatterUtilities.IsPrimitiveType(fieldType);
					Drawer drawer = null;


					if (isUnitySupportedType)
					{
						drawer = new FieldDrawer(field);
					}
					else
					{
						drawer = new DefaultObjectDrawer(field, fieldType); //  GetDrawer(field);
																			//drawer.displayName = field.Name;
					}

					//drawer.displayName = field.Name;
					DrawCommand drawCommand = new DrawCommand(drawer, field, isUnitySupportedType);
					if (drawer.isDrawable)
					{
						this.height += drawer.height;
					}

					drawers.Add(drawCommand);
				}

				return drawers.ToArray();
			}
		}

		/// <summary>
		/// A custom object drawer for a given type
		/// </summary>
		/// <typeparam name="T"></typeparam>
		public abstract class CustomObjectDrawer : ObjectDrawer
		{
			public CustomObjectDrawer(Type type) : base(type)
			{
			}
		}

		/// <summary>
		/// A custom object drawer for a given type
		/// </summary>
		/// <typeparam name="T"></typeparam>
		public abstract class CustomObjectDrawer<T> : CustomObjectDrawer
		{
			//------------------------------------------------------------------------/
			// Virtual
			//------------------------------------------------------------------------/
			protected abstract void OnDrawEditorGUI(Rect position, T value);
			protected abstract void OnDrawEditorGUILayout(T value);
			protected abstract float GetHeight(T value);

			//------------------------------------------------------------------------/
			// CTOR
			//------------------------------------------------------------------------/
			public CustomObjectDrawer() : base(typeof(T))
			{
			}

			//------------------------------------------------------------------------/
			// Messages
			//------------------------------------------------------------------------/
			public override bool DrawEditorGUI(Rect position, object target)
			{
				EditorGUI.BeginChangeCheck();
				this.OnDrawEditorGUI(position, (T)target);
				bool changed = EditorGUI.EndChangeCheck();
				return changed;
			}

			public override bool DrawEditorGUILayout(object target, bool isChild = false)
			{
				EditorGUI.BeginChangeCheck();
				this.OnDrawEditorGUILayout((T)target);
				bool changed = EditorGUI.EndChangeCheck();
				return changed;
			}

		}
	}

}