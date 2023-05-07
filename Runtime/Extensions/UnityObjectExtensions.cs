using System.Collections.Generic;

using UnityEngine;

namespace Stratus.Unity.Extensions
{
	public static class UnityObjectExtensions
	{
		/// <summary>
		/// Checks whether the object is null (exhaustively)
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public static bool IsNull(this Object obj)
		{
			return obj == null || obj.Equals(null);
		}

		/// <summary>
		/// Whether the object is in a valid state
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public static bool IsValid(this Object obj) => !obj.IsNull();

		/// <summary>
		/// Destroys all the objects in the list, then clears it
		/// </summary>
		/// <param name="list"></param>
		public static void DestroyAndClear<T>(this List<T> list)
			where T : Object
		{
			list.ForEach((obj) => obj.Destroy());
			list.Clear();
		}

		/// <summary>
		/// Destroys all the objects in the list, then clears it
		/// </summary>
		/// <param name="list"></param>
		public static void DestroyGameObjectsAndClear<T>(this List<T> list)
			where T : Component
		{
			list.ForEach((obj) => obj.gameObject.Destroy());
			list.Clear();
		}

		/// <summary>
		/// Destroys the Object.
		/// </summary>
		/// <param name="obj"></param>
		public static void DestroyImmediate<T>(this T obj)
			where T : Object
		{
			Object.DestroyImmediate(obj);
		}

		/// <summary>
		/// Destroys the Object.
		/// </summary>
		/// <param name="obj"></param>
		public static void Destroy<T>(this T obj, float t = 0)
			where T : Object
		{
			Object.Destroy(obj, t);
		}

		/// <summary>
		/// Destroys the Object.
		/// </summary>
		/// <param name="obj"></param>
		public static void DestroyGameObject<T>(this T obj, float t = 0)
			where T : Component
		{
			Object.Destroy(obj.gameObject, t);
		}

	}
}