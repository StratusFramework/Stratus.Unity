using UnityEngine;

namespace Stratus.Unity.Extensions
{
	public static class RectTransformExtensions
	{
		public static void SetWidth(this RectTransform rectTransform, float width)
		{
			rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
		}

		public static void SetHeight(this RectTransform rectTransform, float height)
		{
			rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
		}
	}

}