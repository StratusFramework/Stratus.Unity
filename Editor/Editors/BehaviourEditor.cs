using Stratus.Editor;

using System;

using UnityEngine;

namespace Stratus.Unity.Editor
{
	/// <summary>
	/// Base editor for all Stratus components
	/// </summary>
	public abstract class BehaviourEditor<T> : StratusEditor where T : MonoBehaviour
	{
		protected new T target { get; private set; }
		protected override Type baseType => typeof(MonoBehaviour);
		protected virtual void OnBehaviourEditorValidate() { }

		internal override void OnStratusGenericEditorEnable()
		{
			this.SetTarget();
		}

		internal override void OnGenericStratusEditorValidate()
		{
			if (!this.target)
			{
				this.SetTarget();
			}

			if (this.target)
			{
				this.OnBehaviourEditorValidate();
			}
		}

		private void SetTarget()
		{
			this.target = base.target as T;
		}

	}

}