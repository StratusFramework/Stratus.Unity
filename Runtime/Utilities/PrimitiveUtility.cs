using UnityEngine;
using System.Collections.Generic;

// REFERENCE:
// http://answers.unity3d.com/questions/514293/changing-a-gameobjects-primitive-mesh.html

namespace Stratus.Unity.Utility
{
	public static class PrimitiveUtility
	{
		private static Dictionary<PrimitiveType, Mesh> primitiveMeshes = new Dictionary<PrimitiveType, Mesh>();

		/// <summary>
		/// Instantiates a primitive of the given type.
		/// </summary>
		/// <param name="type"></param>
		/// <param name="withCollider"></param>
		/// <returns></returns>
		public static GameObject CreatePrimitive(PrimitiveType type, bool withCollider)
		{
			if (withCollider) { return GameObject.CreatePrimitive(type); }

			GameObject gameObject = new GameObject(type.ToString());
			MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>();
			meshFilter.sharedMesh = GetPrimitiveMesh(type);
			gameObject.AddComponent<MeshRenderer>();

			return gameObject;
		}

		public static Mesh GetPrimitiveMesh(PrimitiveType type)
		{
			if (!primitiveMeshes.ContainsKey(type))
			{
				CreatePrimitiveMesh(type);
			}

			return primitiveMeshes[type];
		}

		private static Mesh CreatePrimitiveMesh(PrimitiveType type)
		{
			GameObject gameObject = GameObject.CreatePrimitive(type);
			Mesh mesh = gameObject.GetComponent<MeshFilter>().sharedMesh;
			Object.DestroyImmediate(gameObject);

			primitiveMeshes[type] = mesh;
			return mesh;
		}
	}
}
