using UnityEngine;
using UnityEngine.Tilemaps;

public class ShadowCasterMover : MonoBehaviour
{
    public Transform player;  // Reference to the player
    public Tilemap tilemap;   // The tilemap attached in the game

    void Update()
    {
        // Get the player's world position
        Vector3 playerWorldPosition = player.position;

        // Convert the world position to cell position (grid coordinates)
        Vector3Int cellPosition = tilemap.WorldToCell(playerWorldPosition);

        // Convert the cell position back to world position
        Vector3 worldPosition = tilemap.CellToWorld(cellPosition) + tilemap.tileAnchor;

        // Update the shadow caster position
        transform.position = new Vector3(worldPosition.x, worldPosition.y, transform.position.z);
    }
}

