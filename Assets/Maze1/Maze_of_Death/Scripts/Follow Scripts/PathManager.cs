using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathManager : MonoBehaviour
{
    public TilemapPathfinding pathfinder;

    // ✅ Updated to cache using both start and end positions
    private Dictionary<(Vector3Int start, Vector3Int end), List<Vector3>> cachedPaths = new();

    public static PathManager Instance;

    void Awake() => Instance = this;

    public void RequestPath(Vector3 startWorld, Vector3 targetWorld, System.Action<List<Vector3>> callback)
    {
        Vector3Int startCell = pathfinder.grid.WorldToCell(startWorld);
        Vector3Int targetCell = pathfinder.grid.WorldToCell(targetWorld);

        var key = (startCell, targetCell);

        if (cachedPaths.TryGetValue(key, out var cachedPath))
        {
            callback?.Invoke(cachedPath);
        }
        else
        {
            StartCoroutine(GeneratePath(startWorld, targetWorld, key, callback));
        }
    }

    private IEnumerator GeneratePath(Vector3 startWorld, Vector3 targetWorld, (Vector3Int start, Vector3Int end) key, System.Action<List<Vector3>> callback)
    {
        List<Vector3> path = null;
        yield return StartCoroutine(pathfinder.FindPathAsync(startWorld, targetWorld, result => path = result));

        // ✅ Cache only if a valid path was found
        if (path != null && path.Count > 0)
            cachedPaths[key] = path;

        callback?.Invoke(path);
    }

    public void ClearCache()
    {
        cachedPaths.Clear();
    }
}
