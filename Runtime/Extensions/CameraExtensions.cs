using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.InputSystem;

namespace Stratus.Unity.Extensions
{
	public static class CameraExtensions
	{
		/// <summary>
		/// Returns the world position of the first point hit by a ray cast from the
		/// mouse's current position on the screen.
		/// </summary>
		/// <param name="camera"></param>
		/// <returns>The point at which the ray cast by the mouse's position on the screen hit a transform on the world.</returns>
		public static RaycastHit CastRayFromMouseScreenPosition(this Camera camera)
		{
			var ray = camera.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;
			Physics.Raycast(ray, out hit);
			return hit;
		}

		/// <summary>
		/// Returns the world position of the first point hit by a ray cast from the
		/// mouse's current position on the screen. If nothing was hit, returns the zero vector.
		/// </summary>
		/// <param name="camera"></param>
		/// <returns>The point at which the ray cast by the mouse's position on the screen hit a transform on the world.</returns>
		public static Vector3 MouseCastGetPosition(this Camera camera)
		{
			var ray = camera.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;
			if (Physics.Raycast(ray, out hit))
				return hit.point;
			return Vector3.zero;
		}

		public static Vector3 GetMousePositionToWorld(this Camera camera, bool legacy = false)
		{
			return camera.ScreenToWorldPoint(legacy
				? Input.mousePosition
				: Mouse.current.position.ReadValue());
		}

		/// <summary>
		/// Returns the first transform that was hit by casting a ray from the mouse's position to the world
		/// </summary>
		/// <param name="camera"></param>
		/// <returns>The transform that was hit, or null if no hits</returns>
		public static Transform MouseCastGetTransform(this Camera camera)
		{
			var ray = camera.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;
			if (Physics.Raycast(ray, out hit))
				return hit.transform;
			return null;
		}

		/// <summary>
		/// Casts a ray from the camera towards the screen position
		/// </summary>
		/// <param name="camera"></param>
		/// <param name="position"></param>
		/// <returns></returns>
		public static Transform RayCast2DGetTransform(this Camera camera, Vector3 position)
		{
			var hit = Physics2D.Raycast(position, camera.transform.position - position, float.Epsilon);
			return hit.transform;
		}

		/// <summary>
		/// Checks whether a given transform is visible by the camera
		/// </summary>
		/// <param name="target"></param>
		/// <returns>True if the object is visible by the camera, false otherwise</returns>
		public static bool IsInViewport(this Camera camera, Transform target)
		{
			// Convert the target's position from world space to view space
			var viewPos = camera.WorldToViewportPoint(target.position);

			if (viewPos.x < 0f || viewPos.x > 1f)
				return false;
			if (viewPos.y < 0f || viewPos.y > 1f)
				return false;
			// If the z-value is negative, it is behind the camera
			if (viewPos.z < 0f)
				return false;

			return true;
		}

		/// <summary>
		/// Compares whether the first object is to the left of the other within the camera viewport
		/// </summary>
		/// <param name="camera"></param>
		/// <param name="first"></param>
		/// <param name="second"></param>
		/// <returns></returns>
		public static bool IsToTheLeftOf(this Camera camera, Transform first, Transform second)
		{
			var firstViewPos = camera.WorldToViewportPoint(first.position);
			var secondViewPos = camera.WorldToViewportPoint(second.position);
			if (firstViewPos.x < secondViewPos.x)
				return true;
			return false;
		}

		/// <summary>
		/// Orders the given gameobjects by their positions within the camera's view space, from left to right.
		/// </summary>
		/// <param name="camera"></param>
		/// <param name="targets"></param>
		/// <returns></returns>
		public static GameObject[] OrderByHorizontalPosition(this Camera camera, GameObject[] targets)
		{
			var sortedPositions = new List<GameObject>(targets);
			sortedPositions.Sort((a, b) => camera.WorldToViewportPoint(b.transform.position).x.CompareTo(
										   camera.WorldToViewportPoint(a.transform.position).x));
			return sortedPositions.ToArray();
		}

		/// <summary>
		/// Finds all GameObjects with a specified tag that are within view of the camera.
		/// </summary>
		/// <param name="camera"></param>
		/// <param name="tag"></param>
		/// <returns></returns>
		public static GameObject[] FindGameObjectsInView(this Camera camera, string tag)
		{
			var taggedObjects = GameObject.FindGameObjectsWithTag(tag);
			var objectsWithinView = (from GameObject obj in taggedObjects
									 where camera.IsInViewport(obj.transform)
									 select obj).ToArray();
			return objectsWithinView;
		}

		/// <summary>
		/// Given an input direction from an input axis (such as from a joystick), calculates
		/// the vector needed to move the specified transform in that direction relative to the camera.
		/// </summary>
		/// <param name="camera"></param>
		/// <param name="inputAxis"></param>
		/// <returns></returns>
		public static Vector3 CalculateRelativeDirection(this Camera camera, Transform transform, Vector2 inputAxis, StratusVectorAxis axixToIgnore)
		{
			Vector3 cameraToTransform = transform.position - camera.transform.position;
			if (axixToIgnore == StratusVectorAxis.Y)
				cameraToTransform.y = 0;


			var ctpNorm = cameraToTransform.normalized;
			var rightVec = new Vector3(ctpNorm.z, 0, -ctpNorm.x);

			return (ctpNorm * inputAxis.y + rightVec * inputAxis.x).normalized;
		}

		// @TODO: Not finished!
		public static GameObject FindClosestToCenter(this Camera camera, GameObject[] targetsInView)
		{
			GameObject closestTarget = null;
			// We want to look for the target that is closest to 0.5
			foreach (var target in targetsInView)
			{
				var targetInViewSpace = camera.WorldToViewportPoint(target.transform.position);
				// If the z value is negative, it is behind the camera
				if (targetInViewSpace.z < 0f)
					continue;
				var dist = targetInViewSpace.magnitude;
				//if (dist )
			}
			return closestTarget;
		}

		/// <summary>
		/// Calculates the horizontal field of view for the camera. By default the fieldOfView property returns the vertical.
		/// </summary>
		/// <param name="camera"></param>
		/// <returns></returns>
		public static float GetHorizontalFOV(this Camera camera)
		{
			return Mathf.Rad2Deg * 2f * Mathf.Atan(Mathf.Tan(Mathf.Deg2Rad * camera.fieldOfView * 0.5f) * camera.aspect);

		}


	}
}