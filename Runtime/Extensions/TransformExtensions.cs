using System.Collections.Generic;
using System.Linq;

using UnityEngine;

namespace Stratus.Unity.Extensions
{
	public static class TransformExtensions
	{
		/// <summary>
		/// Centers this transform on the parent
		/// </summary>
		/// <param name="transform"></param>
		/// <param name="parent"></param>
		public static void Center(this Transform transform)
		{
			transform.localPosition = Vector3.zero;
		}

		/// <summary>
		/// Resets the translation, rotation and scale of this transform back to default values
		/// </summary>
		/// <param name="transform"></param>
		/// <param name="parent"></param>
		public static void Reset(this Transform transform)
		{
			transform.localPosition = Vector3.zero;
			transform.localScale = Vector3.one;
			transform.rotation = Quaternion.identity;
		}

		/// <summary>
		/// Calculates a position in front of the transform at a given distance
		/// </summary>
		/// <param name="transform"></param>
		/// <param name="distance"></param>
		/// <returns></returns>
		public static Vector3 CalculateForwardPosition(this Transform transform, float distance)
		{
			return transform.position + transform.forward * distance;
		}

		/// <summary>
		/// Calculates a position on a given normalized direction vector from the transform's position.
		/// </summary>
		/// <param name="transform"></param>
		/// <param name="direction"></param>
		/// <param name="distance"></param>
		/// <returns></returns>
		public static Vector3 CalculatePositionAtDirection(this Transform transform, Vector3 direction, float distance)
		{
			return transform.position + direction * distance;
		}

		/// <summary>
		/// Gets a list of all transforms within range
		/// </summary>
		/// <param name="transform"></param>
		/// <param name="radius"></param>
		/// <returns></returns>
		public static Transform[] GetTransformsWithinRadius(this Transform transform, float radius)
		{
			Collider[] hits = Physics.OverlapSphere(transform.position, radius);
			Transform[] transforms = hits.Select(x => x.transform).ToArray();
			return transforms;
		}

		/// <summary>
		/// Finds the child of this transform, using Breadth-first search
		/// </summary>
		/// <param name="parent"></param>
		/// <param name="name"></param>
		/// <returns></returns>
		public static Transform FindChildBFS(this Transform parent, string name)
		{
			Transform result = parent.Find(name);
			if (result != null)
			{
				return result;
			}

			foreach (Transform child in parent)
			{
				result = child.FindChildBFS(name);
				if (result != null)
				{
					return result;
				}
			}
			return null;
		}

		/// <summary>
		/// Returns a container of all the children of this transform.
		/// </summary>
		/// <param name="transform"></param>
		/// <returns>A container of all the children of this transform.</returns>
		public static Transform[] Children(this Transform transform)
		{
			void listChildren(Transform obj, List<Transform> children)
			{
				foreach (Transform child in obj.transform)
				{
					children.Add(child);
					listChildren(child, children);
				}
			}
			List<Transform> _children = new List<Transform>();
			listChildren(transform, _children);
			return _children.ToArray();
		}



	}
}
