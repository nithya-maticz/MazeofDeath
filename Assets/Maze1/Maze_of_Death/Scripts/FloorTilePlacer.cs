using UnityEngine;
using UnityEngine.Tilemaps;

public class FloorTileFiller : MonoBehaviour
{
    public Tilemap sourceTilemap;   // Grid A (with walls/objects)
    public Tilemap targetTilemap;   // Grid B (empty floor grid)
    public TileBase floorTile;      // Floor tile to place

    public void FillInsideBounds()
    {
        if (sourceTilemap == null || targetTilemap == null || floorTile == null)
        {
            Debug.LogError("Assign Source, Target Tilemap, and FloorTile in inspector!");
            return;
        }

        targetTilemap.ClearAllTiles(); // optional

        // Get bounds of source
        BoundsInt bounds = sourceTilemap.cellBounds;
        TileBase[] allTiles = sourceTilemap.GetTilesBlock(bounds);

        // Determine min and max occupied cell
        Vector3Int min = new Vector3Int(int.MaxValue, int.MaxValue, 0);
        Vector3Int max = new Vector3Int(int.MinValue, int.MinValue, 0);

        for (int x = 0; x < bounds.size.x; x++)
        {
            for (int y = 0; y < bounds.size.y; y++)
            {
                TileBase tile = allTiles[x + y * bounds.size.x];
                if (tile != null)
                {
                    int worldX = bounds.x + x;
                    int worldY = bounds.y + y;

                    if (worldX < min.x) min.x = worldX;
                    if (worldY < min.y) min.y = worldY;
                    if (worldX > max.x) max.x = worldX;
                    if (worldY > max.y) max.y = worldY;
                }
            }
        }

        // Fill entire rectangle area in targetTilemap
        for (int x = min.x; x <= max.x; x++)
        {
            for (int y = min.y; y <= max.y; y++)
            {
                targetTilemap.SetTile(new Vector3Int(x, y, 0), floorTile);
            }
        }

        Debug.Log($"Filled floor from {min} to {max}");
    }
}
