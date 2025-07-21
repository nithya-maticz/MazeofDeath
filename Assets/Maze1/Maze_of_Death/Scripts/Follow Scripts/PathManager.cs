using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathManager : MonoBehaviour
{
    public TilemapPathfinding pathfinder;

    private Dictionary<(Vector3Int start, Vector3Int end), List<Vector3>> cachedPaths = new();

    private Queue<PathRequest> requestQueue = new();
    private bool isProcessing = false;

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
            requestQueue.Enqueue(new PathRequest(startWorld, targetWorld, key, callback));
            if (!isProcessing)
                StartCoroutine(ProcessQueue());
        }
    }

    private IEnumerator ProcessQueue()
    {
        isProcessing = true;

        while (requestQueue.Count > 0)
        {
            var request = requestQueue.Dequeue();
            List<Vector3> path = null;
            yield return StartCoroutine(pathfinder.FindPathAsync(request.startWorld, request.targetWorld, result => path = result));

            if (path != null && path.Count > 0)
                cachedPaths[request.key] = path;

            request.callback?.Invoke(path);
            yield return null; // prevent frame spike
        }

        isProcessing = false;
    }

    public void ClearCache()
    {
        cachedPaths.Clear();
    }

    private struct PathRequest
    {
        public Vector3 startWorld;
        public Vector3 targetWorld;
        public (Vector3Int, Vector3Int) key;
        public System.Action<List<Vector3>> callback;

        public PathRequest(Vector3 s, Vector3 t, (Vector3Int, Vector3Int) k, System.Action<List<Vector3>> c)
        {
            startWorld = s;
            targetWorld = t;
            key = k;
            callback = c;
        }
    }
}