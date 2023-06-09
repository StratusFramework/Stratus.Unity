using System;
using System.Collections.Generic;
using System.Reflection;

using UnityEngine;

namespace Stratus.Unity.Extensions
{
	public static class GameObjectExtensions
	{
		/// <summary>
		/// Returns a container of all the children of this GameObject.
		/// </summary>
		/// <param name="gameObj"></param>
		/// <returns>A container of all the children of this GameObject.</returns>
		public static List<GameObject> Children(this GameObject gameObj)
		{
			void listChildren(GameObject obj, List<GameObject> children)
			{
				foreach (Transform child in obj.transform)
				{
					children.Add(child.gameObject);
					listChildren(child.gameObject, children);
				}
			}

			List<GameObject> _children = new List<GameObject>();
			listChildren(gameObj, _children);
			return _children;
		}

		/// <summary>
		/// Finds the child of this GameObject with a given name
		/// </summary>
		/// <param name="gameObj"></param>
		/// <param name="name"></param>
		/// <returns></returns>
		public static GameObject FindChild(this GameObject gameObj, string name)
		{
			return TransformExtensions.FindChildBFS(gameObj.transform, name).gameObject;
		}

		/// <summary>
		/// Returns the parent GameObject of this GameObject.
		/// </summary>
		/// <param name="gameObj"></param>
		/// <returns>A reference to the GameObject parent of this GameObject.</returns>
		public static GameObject Parent(this GameObject gameObj)
		{
			return gameObj.transform.parent.gameObject;
		}

		/// <summary>
		/// Adds a component to this GameObject, through copying an existing one.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="gameObj"></param>
		/// <param name="componentToCopy">The component to copy.</param>
		/// <returns>A reference to the newly added component.</returns>
		public static T AddComponent<T>(this GameObject gameObj, T componentToCopy) where T : Component
		{
			return gameObj.AddComponent<T>().CopyFrom(componentToCopy);
		}

		/// <summary>
		/// Checks whether this GameObject has the specified component.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="gameObj"></param>
		/// <returns>True if the component was present, false otherwise.</returns>
		public static bool HasComponent<T>(this GameObject gameObj) where T : Component
		{
			return gameObj.GetComponent<T>() != null;
		}

		/// <summary>
		/// Gets or if not present, adds the specified component to the GameObject.
		/// </summary>
		public static T GetOrAddComponent<T>(this GameObject go) where T : Component
		{
			T component = go.GetComponent<T>();
			if (component == null)
			{
				component = go.gameObject.AddComponent<T>();
			}

			return component;
		}

		/// <summary>
		/// Gets or if not present, adds the specified component to the GameObject.
		/// </summary>
		public static Component GetOrAddComponent(this GameObject go, Type type)
		{
			Component component = go.GetComponent(type);
			if (component == null)
			{
				component = go.gameObject.AddComponent(type);
			}

			return component;
		}

		public static bool RemoveComponent<T>(this GameObject gameObject) where T : Component
		{
			T component = gameObject.GetComponent<T>();
			if (component != null)
			{
				if (Application.isPlaying)
				{
					UnityEngine.Object.Destroy(component);
				}
				else if (Application.isEditor)
				{
					UnityEngine.Object.DestroyImmediate(component);
				}

				return true;
			}
			return false;
		}

		public static bool RemoveComponent(this GameObject gameObject, Type type)
		{
			Component component = gameObject.GetComponent(type);
			if (component != null)
			{
				if (Application.isPlaying)
				{
					UnityEngine.Object.Destroy(component);
				}
				else if (Application.isEditor)
				{
					UnityEngine.Object.DestroyImmediate(component);
				}

				return true;
			}
			return false;
		}

		public static void RemoveComponents(this GameObject gameObject, params Type[] types)
		{
			foreach (Type type in types)
			{
				gameObject.RemoveComponent(type);
			}
		}

		/// <summary>
		/// Duplicates the given component on this GameObject
		/// </summary>
		/// <param name="gameObject"></param>
		/// <param name="component"></param>
		/// <returns></returns>
		public static Component DuplicateComponent(this GameObject gameObject, Component component, bool copy = false)
		{
			Type type = component.GetType();
			Component newComponent = gameObject.AddComponent(type);
			if (copy)
			{
				newComponent.CopyFrom(component);
			}

			return newComponent;
		}

		/// <summary>
		/// Duplicates the given component onto this GameObject. All its values will be copied over into the copy.
		/// </summary>
		/// <param name="gameObject"></param>
		/// <param name="component"></param>
		/// <returns></returns>
		public static T DuplicateComponent<T>(this GameObject gameObject, T component) where T : Component
		{
			T newComponent = gameObject.AddComponent<T>();
			newComponent.CopyFrom(component);
			return newComponent;
		}

		/// <summary>
		/// Finds and invokes the method with the given name among all components
		/// attached to this GameObject
		/// </summary>
		/// <param name="go"></param>
		/// <param name="methodName"></param>
		/// <returns></returns>
		public static void FindAndInvokeMethod(this GameObject go, string methodName)
		{
			MonoBehaviour[] components = go.GetComponents<MonoBehaviour>();
			MethodInfo mInfo = components.GetType().GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Instance);
			if (mInfo != null)
			{
				mInfo.Invoke(components, null);
			}
		}

	}
}