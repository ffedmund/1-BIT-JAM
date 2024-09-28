using UnityEngine;
using UnityEngine.Tilemaps;

public class ShadowBlockCreator : MonoBehaviour
{
    public Transform lightSource;        // Reference to the light source
    public Tilemap tilemap;              // The tilemap to place blocks on
    public TileBase solidTile;           // The tile to use for solid blocks
    public Transform player;             // Reference to the player
    public LayerMask layerMask;          // Layer mask to filter raycast hits
    public Camera mainCamera;            // Reference to the main camera

    private Bounds cameraBounds;

    void Update()
    {
        // Update camera bounds for this frame
        cameraBounds = OrthographicBounds(mainCamera);

        // Check if the light source is within the camera bounds (2D check, ignoring Z)
        Vector3 lightPos2D = new Vector3(lightSource.position.x, lightSource.position.y, 0);
        if (!cameraBounds.Contains(lightPos2D))
        {
            // Light source is out of camera view, clear shadows and exit
            ClearShadows();
            return;
        }

        // Calculate the player's tile position based on the light source's position
        Vector3Int playerTilePos = GetRoundedPlayerTilePosition();

        // Light source is within camera view
        Vector3Int minTile = tilemap.WorldToCell(cameraBounds.min);
        Vector3Int maxTile = tilemap.WorldToCell(cameraBounds.max);

        for (int x = minTile.x; x <= maxTile.x; x++)
        {
            for (int y = minTile.y; y <= maxTile.y; y++)
            {
                Vector3Int tilePos = new Vector3Int(x, y, 0);
                if (tilePos == playerTilePos)
                {
                    // Skip the player's tile position
                    if (tilemap.GetTile(tilePos) != null)
                    {
                        tilemap.SetTile(tilePos, null);
                    }
                    continue;
                }

                Vector3 worldPos = tilemap.CellToWorld(tilePos) + tilemap.tileAnchor;
                Vector2 lightSourcePos2D = new Vector2(lightSource.position.x, lightSource.position.y);
                Vector2 worldPos2D = new Vector2(worldPos.x, worldPos.y);
                Vector2 direction = (worldPos2D - lightSourcePos2D).normalized;
                float distanceToTile = Vector2.Distance(lightSourcePos2D, worldPos2D);

                // Cast a ray from the light source to the tile position
                RaycastHit2D hit = Physics2D.Raycast(lightSourcePos2D, direction, distanceToTile, layerMask);

                if (hit.collider != null && hit.collider.transform == player)
                {
                    // Player is casting a shadow on this tile
                    if (tilemap.GetTile(tilePos) == null)
                    {
                        tilemap.SetTile(tilePos, solidTile);
                    }
                }
                else
                {
                    // No shadow; remove the tile if it exists
                    if (tilemap.GetTile(tilePos) != null)
                    {
                        tilemap.SetTile(tilePos, null);
                    }
                }
            }
        }
    }

    // Method to get the orthographic bounds of the camera view
    private Bounds OrthographicBounds(Camera camera)
    {
        float screenAspect = (float)Screen.width / Screen.height;
        float cameraHeight = camera.orthographicSize * 2;
        return new Bounds(
            new Vector3(camera.transform.position.x, camera.transform.position.y, 0),
            new Vector3(cameraHeight * screenAspect, cameraHeight, 0));
    }

    // Method to clear shadow tiles within the camera view bounds
    private void ClearShadows()
    {
        Vector3Int minTile = tilemap.WorldToCell(cameraBounds.min);
        Vector3Int maxTile = tilemap.WorldToCell(cameraBounds.max);

        for (int x = minTile.x; x <= maxTile.x; x++)
        {
            for (int y = minTile.y; y <= maxTile.y; y++)
            {
                Vector3Int tilePos = new Vector3Int(x, y, 0);
                if (tilemap.GetTile(tilePos) != null)
                {
                    tilemap.SetTile(tilePos, null);
                }
            }
        }
    }

    // Method to get the player's rounded tile position based on light source
    private Vector3Int GetRoundedPlayerTilePosition()
    {
        Vector3 playerPosition = player.position;
        Vector3Int playerTilePos;

        playerTilePos = new Vector3Int(
            Mathf.FloorToInt(playerPosition.x),
            Mathf.FloorToInt(playerPosition.y),
            0);

        // Convert world position to tile position
        return tilemap.WorldToCell(playerTilePos);
    }
}
