using System.Collections;

using UnityEngine;

namespace Stratus.Unity.Routines
{
	public static class RigidbodyRoutines
	{
		public static IEnumerator AddForce(Rigidbody rigidBody, Vector3 force, ForceMode mode, float duration)
		{
			rigidBody.isKinematic = false;
			float timeElapsed = 0f;
			while (timeElapsed <= duration)
			{
				timeElapsed += Time.deltaTime;
				rigidBody.AddForce(force, mode);
				yield return new WaitForFixedUpdate();
			}
			rigidBody.isKinematic = true;
		}

		public static IEnumerator AddImpulse(Rigidbody rigidBody, Vector3 force, System.Action onFinished = null)
		{
			bool isKinematic = rigidBody.isKinematic;
			rigidBody.isKinematic = false;
			rigidBody.AddForce(force, ForceMode.Impulse);
			///Trace.Script("Started");

			yield return new WaitForSeconds(0.1f);

			while (rigidBody.velocity != Vector3.zero)
			{
				//Trace.Script($" velocity = {rigidBody.velocity}");
				yield return new WaitForFixedUpdate();
			}

			//Trace.Script("Ended");
			rigidBody.isKinematic = isKinematic;
			onFinished?.Invoke();
		}
	}
}
