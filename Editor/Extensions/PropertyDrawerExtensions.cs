using Stratus.Reflection;

using System;
using System.Reflection;

using UnityEditor;

using UnityEngine;

namespace Stratus.Unity.Editor
{
	public static partial class PropertyDrawerExtensions
	{
		public static float GetPropertyHeightSafe(this PropertyDrawer drawer, SerializedProperty property, GUIContent label)
		{
			return (float)ReflectionUtility.GetReflectedMethod("GetPropertyHeightSafe", typeof(PropertyDrawer), false, drawer).Invoke(drawer, new object[] { property, label });
		}

		public static void OnGUISafe(this PropertyDrawer drawer, Rect position, SerializedProperty property, GUIContent label)
		{
			ReflectionUtility.GetReflectedMethod("OnGUISafe", typeof(PropertyDrawer), false, drawer).Invoke(drawer, new object[] { position, property, label });
		}

		public static void SetFieldInfo(this PropertyDrawer drawer, FieldInfo info)
		{
			ReflectionUtility.SetField("m_FieldInfo", typeof(PropertyDrawer), info, false, drawer);
		}

		public static void SetAttribute(this PropertyDrawer drawer, PropertyAttribute attrib)
		{
			ReflectionUtility.SetField("m_Attribute", typeof(PropertyDrawer), attrib, false, drawer);
		}

		public static void SetAttribute(this DecoratorDrawer drawer, PropertyAttribute attrib)
		{
			ReflectionUtility.SetField("m_Attribute", typeof(DecoratorDrawer), attrib, false, drawer);
		}

		public static Type GetHiddenType(this CustomPropertyDrawer prop)
		{
			return ReflectionUtility.GetField<Type>("m_Type", typeof(CustomPropertyDrawer), false, prop);
		}

		public static bool GetUseForChildren(this CustomPropertyDrawer prop)
		{
			return ReflectionUtility.GetField<bool>("m_UseForChildren", typeof(CustomPropertyDrawer), false, prop);
		}

	}
}