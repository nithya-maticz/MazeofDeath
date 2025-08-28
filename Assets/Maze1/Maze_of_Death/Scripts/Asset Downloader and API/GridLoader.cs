using System.Collections.Generic;
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

    private GameObject backgroundInstance;
    private readonly List<GameObject> spawnedPrefabs = new List<GameObject>();

    private void Awake()
    {
        Instance = this;
    }

    public void LoadLevel()
    {
        ClearOldLevel();

        LoadTile();
        LoadPrefab();
        SpawnBackgrounds(bgSprite);
        SpawnPatrolPoints();
        SpawnPlayer();

        StartCoroutine(Game_Manager.Instance.StartData());
        followCamera.enabled = true;
    }

    /// <summary>
    /// Destroy previously spawned objects when reloading the scene.
    /// </summary>
    private void ClearOldLevel()
    {
        // Clear prefabs
        foreach (var obj in spawnedPrefabs)
        {
            if (obj != null) Destroy(obj);
        }
        spawnedPrefabs.Clear();

        // Clear patrols
        foreach (var patrol in patrolObjects)
        {
            if (patrol != null) Destroy(patrol);
        }
        patrolObjects.Clear();
        Game_Manager.Instance.PatrolPoints.Clear();

        // Clear background
        if (backgroundInstance != null)
        {
            Destroy(backgroundInstance);
            backgroundInstance = null;
        }

        // Reset tilemap
        wallTilemap.ClearAllTiles();
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
                    (flipX, flipY) = (flipY, flipX);
                    break;
                case 270:
                    (flipX, flipY) = (flipY, flipX);
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
                    Vector3Int cellPos = new Vector3Int(p.position.x, p.position.y, 0);
                    Vector3 worldPos = wallTilemap.CellToWorld(cellPos);
                    worldPos += wallTilemap.cellSize / 2;

                    Quaternion rotation = Quaternion.Euler(0, 0, p.rotation);

                    var instance = Instantiate(prefab, worldPos, rotation);
                    spawnedPrefabs.Add(instance);

                    Debug.Log($"Spawned prefab '{prefab.name}' at {p.position} with rotation {p.rotation}°");
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

        // Destroy old background if somehow still exists
        if (backgroundInstance != null)
        {
            Destroy(backgroundInstance);
        }

        backgroundInstance = new GameObject("Background");
        SpriteRenderer renderer = backgroundInstance.AddComponent<SpriteRenderer>();
        renderer.sprite = bgSprite;
        renderer.sortingOrder = -100;

        renderer.material = Game_Manager.Instance.litMat;

        Bounds bounds = wallTilemap.localBounds;
        float width = bounds.size.x;
        float height = bounds.size.y;

        Vector2 spriteSize = bgSprite.bounds.size;
        backgroundInstance.transform.localScale = new Vector3(
            width / spriteSize.x,
            height / spriteSize.y,
            1
        );

        backgroundInstance.transform.position = bounds.center;

        followCamera.boundsRenderer = renderer;
    }

    void SpawnPatrolPoints()
    {
        foreach (var pos in patrolPoints)
        {
            Vector3Int cellPos = new Vector3Int(pos.x, pos.y, 0);
            Vector3 worldPos = wallTilemap.CellToWorld(cellPos) + wallTilemap.cellSize / 2;

            GameObject go = new GameObject($"PatrolPoint_{pos.x}_{pos.y}");
            go.transform.position = worldPos;

            patrolObjects.Add(go);
            Game_Manager.Instance.PatrolPoints.Add(go.transform);
        }

        Debug.Log($"Spawned {patrolObjects.Count} patrol point objects.");
    }

    void SpawnPlayer()
    {
        Vector3Int cellPos = new Vector3Int(playerSpawn.x, playerSpawn.y, 0);
        Vector3 worldPos = wallTilemap.CellToWorld(cellPos) + wallTilemap.cellSize / 2;

        Game_Manager.Instance.SpawnPlayer(worldPos);
    }
}
