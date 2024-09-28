using UnityEngine;
using UnityEngine.Tilemaps;

public class ShadowCasterMover : MonoBehaviour
{
    public Transform player;              // Reference to the player
    public Tilemap tilemap;               // The tilemap attached in the game


    void Update()
    {
        // Calculate the rounded-off integer position of the player's position
        Vector3 playerPosition = player.position;
        Vector3Int roundedTilePosition = new Vector3Int(
            Mathf.FloorToInt(playerPosition.x),
            Mathf.FloorToInt(playerPosition.y),
            0
        );

        // Convert the tile position to world position
        Vector3 worldPosition = tilemap.CellToWorld(roundedTilePosition) + tilemap.tileAnchor;

        // Update the shadow caster position
        transform.position = new Vector3(worldPosition.x, worldPosition.y, transform.position.z);
    }
}
