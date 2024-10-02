using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.Rendering.Universal;
using System.Collections.Generic;

public class ShadowBlockCreator : MonoBehaviour
{
    public Transform lightSource;        // Reference to the light source
    public Tilemap tilemap;              // The tilemap to place blocks on
    public TileBase solidTile;           // The tile to use for solid blocks
    public Transform player;             // Reference to the player
    public LayerMask layerMask;          // Layer mask to filter raycast hits
    public Camera mainCamera;            // Reference to the main camera

    // New variables for customizable light range
    public bool useCustomLightRange = false;     // Boolean to control which light range to use
    public float customLightOuterRadius = 5f;    // Custom outer radius
    public float customLightInnerRadius = 3f;    // Custom inner radius

    private Light2D light2D;
    private float lightOuterRadius;       // The outer radius of the light
    private float lightInnerRadius;       // The inner radius of the light
    private Bounds cameraBounds;
    private PlayerMovement playerMovement;
    private List<Vector3Int> shadowTiles = new List<Vector3Int>();
    private const int shadowCasterLayer = 7;

    void Start()
    {
        // Get the Light2D component from the light source
        light2D = lightSource.GetComponent<Light2D>();
        playerMovement = player.GetComponent<PlayerMovement>();
        if (light2D != null)
        {
            lightOuterRadius = light2D.pointLightOuterRadius; // Fetch the outer radius
            lightInnerRadius = light2D.pointLightInnerRadius;
        }
        else
        {
            Debug.LogError("Light2D component not found on the light source.");
        }
    }

    void Update()
    {
        // Update camera bounds for this frame
        cameraBounds = OrthographicBounds(mainCamera);

        // Determine which radii to use
        float currentLightOuterRadius = useCustomLightRange ? customLightOuterRadius : lightOuterRadius;
        float currentLightInnerRadius = useCustomLightRange ? customLightInnerRadius : lightInnerRadius;

        // Check if the player is within the light's outer radius
        Vector2 playerPos2D = new Vector2(player.position.x, player.position.y);
        Vector2 lightSourcePos2D = new Vector2(lightSource.position.x, lightSource.position.y);
        float distanceToPlayer = Vector2.Distance(playerPos2D, lightSourcePos2D);

        if (distanceToPlayer > currentLightOuterRadius)
        {
            // Player is out of light's outer radius, clear shadows and exit
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

                // Skip the player's tile position and any tile within the light radius
                Vector3 worldPos = tilemap.CellToWorld(tilePos) + tilemap.tileAnchor;
                Vector2 worldPos2D = new Vector2(worldPos.x, worldPos.y);
                float distanceToTile = Vector2.Distance(lightSourcePos2D, worldPos2D);

                if (tilePos == playerTilePos || distanceToTile <= currentLightInnerRadius)
                {
                    if (tilemap.GetTile(tilePos) != null)
                    {
                        tilemap.SetTile(tilePos, null);
                    }
                    continue;
                }

                Vector2 direction = (worldPos2D - lightSourcePos2D).normalized;

                // Cast a ray from the light source to the tile position
                RaycastHit2D hit = Physics2D.Raycast(lightSourcePos2D, direction, distanceToTile, layerMask);

                if (hit.collider != null && hit.collider.gameObject.layer == shadowCasterLayer)
                {
                    // Player is casting a shadow on this tile
                    if (tilemap.GetTile(tilePos) == null)
                    {
                        tilemap.SetTile(tilePos, solidTile);
                        shadowTiles.Add(tilePos);
                    }
                }
                else
                {
                    // No shadow; remove the tile if it exists
                    if (tilemap.GetTile(tilePos) != null)
                    {
                        tilemap.SetTile(tilePos, null);
                        shadowTiles.RemoveAll(x => x.Equals(tilePos));
                    }
                }
            }
        }

        // Ensure no shadow tiles in front of the player's moving direction
        ClearShadowInFrontOfPlayer();
    }

    void LateUpdate()
    {
        // Ensure no tiles are set within the light radius
        SweepLightRadius();
    }

    private void SweepLightRadius()
    {
        // Determine which inner radius to use
        float currentLightInnerRadius = useCustomLightRange ? customLightInnerRadius : lightInnerRadius;

        Vector2 lightSourcePos2D = new Vector2(lightSource.position.x, lightSource.position.y);
        Vector3Int lightCellPos = tilemap.WorldToCell(lightSourcePos2D);
        int radius = Mathf.CeilToInt(currentLightInnerRadius / tilemap.cellSize.x);

        for (int x = -radius; x <= radius; x++)
        {
            for (int y = -radius; y <= radius; y++)
            {
                Vector3Int tilePos = new Vector3Int(lightCellPos.x + x, lightCellPos.y + y, 0);
                Vector3 worldPos = tilemap.CellToWorld(tilePos) + tilemap.tileAnchor;
                Vector2 worldPos2D = new Vector2(worldPos.x, worldPos.y);
                float distanceToTile = Vector2.Distance(lightSourcePos2D, worldPos2D);
                if (distanceToTile < currentLightInnerRadius && tilemap.GetTile(tilePos) != null)
                {
                    tilemap.SetTile(tilePos, null);
                }
            }
        }
    }

    private void ClearShadowInFrontOfPlayer()
    {
        Vector2 playerVelocity = playerMovement?.GetMovement() ?? Vector2.zero;
        if (playerVelocity.sqrMagnitude > 0.01f) // A small threshold to ensure the player is moving
        {
            Vector2 moveDirection = playerVelocity.normalized;
            moveDirection = moveDirection.y < 0 ? new Vector2(moveDirection.x, 0) : moveDirection;
            Vector2 playerPos = player.position;
            Vector2 frontPos = playerPos + moveDirection * tilemap.cellSize.x; // Check one tile ahead

            Vector3Int frontTilePos = tilemap.WorldToCell(frontPos);

            if (tilemap.GetTile(frontTilePos) != null)
            {
                tilemap.SetTile(frontTilePos, null);
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
        foreach (Vector3Int tilePos in shadowTiles)
        {
            if (tilemap?.GetTile(tilePos) != null)
            {
                tilemap.SetTile(tilePos, null);
            }
        }

        // Clear the list of shadow tiles
        shadowTiles.Clear();
    }

    // Method to get the player's rounded tile position based on light source
    private Vector3Int GetRoundedPlayerTilePosition()
    {
        Vector3 playerPosition = player.position;

        Vector3Int playerTilePos = tilemap.WorldToCell(playerPosition);

        return playerTilePos;
    }

    private void OnDisable() {
        ClearShadows();
    }
}
