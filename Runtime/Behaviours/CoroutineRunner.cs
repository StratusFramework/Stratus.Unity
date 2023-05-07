using System.Collections;

using UnityEngine;

namespace Stratus.Unity.Behaviours
{
	public class CoroutineRunner : SingletonBehaviour<CoroutineRunner>
	{
		public static Coroutine Run(IEnumerator routine)
		{
			return instance.StartCoroutine(routine);
		}

		protected override void OnAwake()
		{
		}
	}

}