using System.Collections.Generic;

using TMPro;

using UnityEngine;

namespace Stratus.Unity.Tilemaps
{
	[RequireComponent(typeof(IMapManager))]
	public class GridText : StratusBehaviour
	{
		[SerializeField]
		private TextMeshPro tileTextPrefab;
		[SerializeField]
		private IMapManager grid;

		private Dictionary<Vector3Int, TextMeshPro> tileTextInstances = new Dictionary<Vector3Int, TextMeshPro>();

		private void Reset()
		{
			grid = GetComponent<IMapManager>();
		}

		public void SetTileText(Vector3Int position, string text)
		{
			if (!tileTextInstances.ContainsKey(position))
			{
				TextMeshPro instance = GameObject.Instantiate(tileTextPrefab, grid.current.baseLayer.tilemap.transform);
				Vector3 worldPosition = grid.current.baseLayer.CellCenterToWorld(position);
				instance.transform.position = worldPosition;
				tileTextInstances.Add(position, instance);
			}

			tileTextInstances[position].gameObject.SetActive(true);
			tileTextInstances[position].text = text;
		}

		public void HideTileText(Vector3Int position)
		{
			if (!tileTextInstances.ContainsKey(position))
			{
				TextMeshPro instance = tileTextInstances[position];
				instance.gameObject.SetActive(false);
			}
		}

		public void ClearTileText(bool destroy = false)
		{
			foreach (var text in tileTextInstances.Values)
			{
				if (destroy)
				{
					Destroy(text.gameObject);
				}
				else
				{
					text.gameObject.SetActive(false);
				}
			}
			if (destroy)
			{
				tileTextInstances.Clear();
			}
		}
	}
}
