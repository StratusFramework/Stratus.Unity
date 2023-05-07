using Stratus.Models.Graph;

using UnityEngine;

namespace Stratus.Unity
{
	public class UnitySerializedTree<TreeElementType> : StratusSerializedTree<TreeElementType>, ISerializationCallbackReceiver
		where TreeElementType : TreeElement, new()
	{
		public void OnBeforeSerialize()
		{
		}

		public void OnAfterDeserialize()
		{
			this.BuildRootFromElements();
		}
	}

}