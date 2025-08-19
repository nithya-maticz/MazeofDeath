using UnityEngine;
using UnityEngine.Tilemaps;

public class GridLoader : MonoBehaviour
{
    public static GridLoader Instance;
    public LevelAssetsDatabase database;
    public Tilemap wallTilemap;
   

    private void Awake()
    {
        Instance = this;
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LoadLevel()
    {
        LoadTile();
        LoadPrefab();
    }

    public void LoadTile()
    {
        wallTilemap.ClearAllTiles();

        LevelData data = ApiManager.Instance.levelData.data;

        foreach (var tile in data.tiles)
        {
            Tilemap targetTilemap = wallTilemap;
            Vector3Int cellPos = new Vector3Int(tile.position.x, tile.position.y, 0);

            targetTilemap.SetTile(cellPos, database.tiles[tile.tileIndex]);

            bool flipX = tile.flipX;
            bool flipY = tile.flipY;

            // Adjust flips depending on rotation
            switch ((int)tile.rotation % 360)
            {
                case 90:
                    // swap axes
                    bool temp90 = flipX;
                    flipX = flipY;
                    flipY = temp90;
                    break;

                case 270:
                    // swap axes again
                    bool temp270 = flipX;
                    flipX = flipY;
                    flipY = temp270;
                    break;
            }

            Vector3 scale = new Vector3(flipX ? -1f : 1f, flipY ? -1f : 1f, 1f);
            Quaternion rotation = Quaternion.Euler(0, 0, tile.rotation);

            Matrix4x4 tileTransform = Matrix4x4.TRS(Vector3.zero, rotation, scale);
            targetTilemap.SetTransformMatrix(cellPos, tileTransform);
        }


    }



    public void LoadPrefab()
    {
        LevelData data = ApiManager.Instance.levelData.data;

        foreach (var p in data.prefabs)
        {
            if (p.prefabIndex >= 0 && p.prefabIndex < database.prefabs.Length)
            {
                var prefab = database.prefabs[p.prefabIndex];
                if (prefab != null)
                {
                    // Use wallTilemap to position prefab
                    Vector3Int cellPos = new Vector3Int(p.position.x, p.position.y, 0);
                    Vector3 worldPos = wallTilemap.CellToWorld(cellPos);
                    worldPos += wallTilemap.cellSize / 2;

                    // Apply rotation
                    float angle = p.rotation;
                    Quaternion rotation = Quaternion.Euler(0, 0, -angle);

                    // Spawn prefab
                    var instance = Instantiate(prefab, worldPos, rotation);
                    Debug.Log($"Spawned prefab '{prefab.name}' at {p.position} with rotation {angle}°");
                }
            }
        }
    }


}
