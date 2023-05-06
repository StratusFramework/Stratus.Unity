using UnityEditor.IMGUI.Controls;

namespace Stratus.Unity.Editor
{
	/// <summary>
	/// An extended version of the MultiColumnHeader with added utility functions
	/// </summary>
	public class StratusMultiColumnHeader : MultiColumnHeader
	{
		public StratusMultiColumnHeader(MultiColumnHeaderState state) : base(state)
		{
		}

		public void ToggleColumn(int columnIndex)
		{
			this.ToggleVisibility(columnIndex);
		}

		public void EnableColumn(int columnIndex)
		{
			if (!this.IsColumnVisible(columnIndex))
				this.ToggleVisibility(columnIndex);
		}

		public void DisableColumn(int columnIndex)
		{
			if (this.IsColumnVisible(columnIndex))
				this.ToggleVisibility(columnIndex);
		}
	}
}