using System;

namespace Stratus.Unity.Data
{
	/// <summary>
	/// Field that allows custom methods to be set for an inspector window and run
	/// </summary>
	[Serializable]
	public class RuntimeMethodField
	{
		/// <summary>
		/// The method which this button will invoke
		/// </summary>
		public Action[] methods { get; private set; }

		public RuntimeMethodField(params Action[] methods)
		{
			this.methods = methods;
		}

		public RuntimeMethodField(Action method)
		{
			this.methods = new Action[] { method };
		}
	}
}
