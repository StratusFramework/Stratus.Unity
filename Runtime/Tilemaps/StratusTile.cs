using Stratus.Logging;

using UnityEngine;
using UnityEngine.Tilemaps;

namespace Stratus.Unity.Tilemaps
{
	[CreateAssetMenu(menuName = "Stratus/2D/Tile", fileName = "Stratus Tile")]
    public class StratusTile : Tile, IStratusLogger
    {
        public Vector3Int position { get; private set; }
        public bool hasGameObject => gameObject != null;

        protected virtual void OnRuntimeStartUp() { }
        public override string ToString()
        {
            return $"{name} [{position}]";
        }

        public override bool StartUp(Vector3Int position, ITilemap tilemap, GameObject go)
        {
            this.position = position;
            if (Application.isPlaying)
            {
                OnRuntimeStartUp();
            }
            return base.StartUp(position, tilemap, go);
        }

        protected static T CreateFromPalette<T>(Sprite sprite)
            where T : StratusTile
        {
            var tile = ScriptableObject.CreateInstance<T>();
            tile.sprite = sprite;
            return tile;
        }
    }
}