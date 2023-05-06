﻿using UnityEngine;
using Stratus;
using System.Collections;

namespace Stratus
{
	/// <summary>
	/// Adds extensions to the Transform component allowing it to run Coroutines targeted for it
	/// </summary>
	public static class StratusRoutinesExtensions
	{
		/// <summary>
		/// Runs a routine that changes the rotation of the given transform, cancelling any previous ones that were doing so.
		/// (Provided they were invoked through the same method)
		/// </summary>
		/// <param name="transform"></param>
		/// <param name="routine">A routine that intends to modify the transform's rotation</param>
		public static void Rotate(this Transform transform, IEnumerator routine)
		{
			StratusTransformRoutineRunner driver = transform.gameObject.GetOrAddComponent<StratusTransformRoutineRunner>();
			driver.Rotate(routine);
		}

		/// <summary>
		/// Starts the transformation routine of the specified type. If a callback function is provided,
		/// it will invoke it upon the completion of the routine.
		/// </summary>
		/// <param name="transform"></param>
		/// <param name="transformationRoutine"></param>
		/// <param name="type"></param>
		/// <param name="onFinished"></param>
		public static void StartCoroutine(this Transform transform, IEnumerator transformationRoutine, TransformationType type, System.Action onFinished = null)
		{
			StratusTransformRoutineRunner driver = transform.gameObject.GetOrAddComponent<StratusTransformRoutineRunner>();
			driver.StartTransformation(transformationRoutine, type, onFinished);
		}

		/// <summary>
		/// Stops any transformation routine(s) of the specified type
		/// </summary>
		/// <param name="transform"></param>
		/// <param name="type"></param>
		public static void StopCoroutine(this Transform transform, TransformationType type)
		{
			StratusTransformRoutineRunner driver = transform.gameObject.GetOrAddComponent<StratusTransformRoutineRunner>();
			driver.StopTransformation(type);
		}

		/// <summary>
		/// Manages a coroutine under a specific tag
		/// </summary>
		/// <param name="mb"></param>
		/// <param name="routine"></param>
		/// <param name="tag"></param>
		/// <param name="onFinished"></param>
		public static void StartCoroutine(this MonoBehaviour mb, IEnumerator routine, string tag, System.Action onFinished = null)
		{
			StratusTaggedRoutines.StartCoroutine(mb, routine, tag, onFinished);
		}

		public static void StopTaggedCoroutine(this MonoBehaviour mb, string tag)
		{
			StratusTaggedRoutines.StopCoroutine(mb, tag);
		}


	}
}