using System.Collections.Generic;

using UnityEditor;

using UnityEngine;

namespace Stratus.Unity.Editor
{
	[InitializeOnLoad]
	public static class SelectByMouseScroll
	{
		private static Renderer[] renderers;
		private static int index;

		static SelectByMouseScroll()
		{
			HierarchyUpdate();
			EditorApplication.hierarchyChanged += HierarchyUpdate;
			SceneView.duringSceneGui += HighlightUpdate;
		}

		private static void HierarchyUpdate() { renderers = Object.FindObjectsOfType<Renderer>(); }

		private static void HighlightUpdate(SceneView sceneview)
		{
			if (Event.current.type != EventType.ScrollWheel || !Event.current.alt)
				return;
			var mp = Event.current.mousePosition;
			var ccam = Camera.current;
			var mouseRay = ccam.ScreenPointToRay(new Vector3(mp.x, ccam.pixelHeight - mp.y, 0f));

			index += Event.current.delta.y >= 0f ? -1 : 1;

			var pointedRenderers = new List<Renderer>();
			foreach (Renderer r in renderers)
				if (r.bounds.IntersectRay(mouseRay))
					pointedRenderers.Add(r);

			if (pointedRenderers.Count > 0)
			{
				index = (index + pointedRenderers.Count) % pointedRenderers.Count;
				Selection.objects = new Object[] { pointedRenderers[index].gameObject };
				Event.current.Use();
			}
		}

	}
}

