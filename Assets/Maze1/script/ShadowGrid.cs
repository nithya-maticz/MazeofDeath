using UnityEngine;
using UnityEngine.Tilemaps;
[RequireComponent(typeof(SpriteRenderer))]
public class ShadowGrid : MonoBehaviour
{
    public Tilemap sourceTilemap;         // The main tilemap
    public Tilemap shadowTilemap;         // The tilemap that will hold shadows
    public Vector3Int shadowOffset = new Vector3Int(1, -1, 0); // Offset for shadow

    public Material shadowMaterial;
    public GameObject Shadow;// Shadow material

    void Start()
    {
        CopyTilesWithOffset();
        ApplyShadowMaterial();
    }

    void CopyTilesWithOffset()
    {
        shadowTilemap.ClearAllTiles();

        BoundsInt bounds = sourceTilemap.cellBounds;

        foreach (Vector3Int pos in bounds.allPositionsWithin)
        {
            TileBase tile = sourceTilemap.GetTile(pos);
            if (tile != null)
            {
                shadowTilemap.SetTile(pos + shadowOffset, tile);
            }
        }
    }

    void ApplyShadowMaterial()
    {
        TilemapRenderer renderer = shadowTilemap.GetComponent<TilemapRenderer>();
        if (renderer != null && shadowMaterial != null)
        {
            renderer.material = shadowMaterial;
        }
    }
}
