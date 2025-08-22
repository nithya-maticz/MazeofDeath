using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GridLoader : MonoBehaviour
{
    public static GridLoader Instance;
    public LevelAssetsDatabase database;
    public Tilemap wallTilemap;
    public Sprite bgSprite;
    public Sprite doorBgSprite;
    public SmoothFollowCamera followCamera;
    public List<Vector2Int> patrolPoints;
    public List<GameObject> patrolObjects = new List<GameObject>();
    public Vector2Int playerSpawn;

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
        SpawnBackgrounds(bgSprite);
        SpawnPatrolPoints();
        SpawnPlayer();

        Game_Manager.Instance.StartData();
        followCamera.enabled = true;
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
                    Quaternion rotation = Quaternion.Euler(0, 0, angle);

                    // Spawn prefab
                    var instance = Instantiate(prefab, worldPos, rotation);
                    Debug.Log($"Spawned prefab '{prefab.name}' at {p.position} with rotation {angle}°");
                }
            }
        }
    }


    void SpawnBackgrounds(Sprite bgSprite)
    {
        if (bgSprite == null)
        {
            Debug.LogWarning("⚠️ No background sprite provided.");
            return;
        }

        GameObject go = new GameObject("Background");
        SpriteRenderer renderer = go.AddComponent<SpriteRenderer>();
        renderer.sprite = bgSprite;
        renderer.sortingOrder = -100;

        // ✅ Use lit material if needed
        renderer.material = Game_Manager.Instance.litMat;

        // ✅ Get tilemap bounds
        Bounds bounds = wallTilemap.localBounds;
        float width = bounds.size.x;
        float height = bounds.size.y;

        // Scale background to match tilemap size
        Vector2 spriteSize = bgSprite.bounds.size;
        go.transform.localScale = new Vector3(
            width / spriteSize.x,
            height / spriteSize.y,
            1
        );

        // Position background at tilemap center
        go.transform.position = bounds.center;

        followCamera.boundsRenderer = renderer;
        
    }

    void SpawnPatrolPoints()
    {
        patrolObjects.Clear();
        Game_Manager.Instance.PatrolPoints.Clear();  // Clear old patrols

        

        foreach (var pos in patrolPoints)
        {
            Vector3Int cellPos = new Vector3Int(pos.x, pos.y, 0);
            Vector3 worldPos = wallTilemap.CellToWorld(cellPos) + wallTilemap.cellSize / 2;

            GameObject go = new GameObject($"PatrolPoint_{pos.x}_{pos.y}");
            go.transform.position = worldPos;

            patrolObjects.Add(go);                          // Local list
            Game_Manager.Instance.PatrolPoints.Add(go.transform);     // Global list
        }

        Debug.Log($"Spawned {patrolObjects.Count} patrol point objects.");
    }

    void SpawnPlayer()
    {
       
        
     

        Vector3Int cellPos = new Vector3Int(playerSpawn.x, playerSpawn.y, 0);
        Vector3 worldPos = wallTilemap.CellToWorld(cellPos) + wallTilemap.cellSize / 2;

        /* Instantiate(playerPrefab, worldPos, Quaternion.identity);
         Debug.Log($"Player spawned at {playerSpawn}");*/
        Game_Manager.Instance.SpawnPlayer(worldPos);

    }
}
