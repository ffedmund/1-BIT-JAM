using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider2D))]
public class PushableBlock : MonoBehaviour
{
    public LayerMask obstacleLayer;
    public float moveDuration = 0.2f; // Duration to move one grid unit
    public Grid grid; // Reference to the Grid component

    private bool isMoving = false;

    private void Start()
    {
        // Snap the block to the grid on start
        SnapToGrid();
    }

    /// <summary>
    /// Snaps the block to the nearest grid position.
    /// </summary>
    private void SnapToGrid()
    {
        transform.position = GetSnapPosition(transform.position);
    }

    private Vector3 GetSnapPosition(Vector3 position)
    {
        Vector3 snappedPosition;
        if (grid != null)
        {
            Vector3Int cellPosition = grid.WorldToCell(position);
            snappedPosition = grid.GetCellCenterWorld(cellPosition);
        }
        else
        {
            // If no grid is assigned, round the position to the nearest integer
            float x = Mathf.Round(position.x);
            float y = Mathf.Round(position.y);
            snappedPosition = new Vector3(x, y, position.z);
        }

        return snappedPosition;
    }

    /// <summary>
    /// Attempts to move the block in the given direction.
    /// </summary>
    /// <param name="direction">The direction to move the block.</param>
    /// <returns>True if the block moved, false otherwise.</returns>
    public bool TryMove(Vector2 direction)
    {
        if (isMoving)
            return false;

        // Calculate the new position
        Vector2 newPos = GetSnapPosition((Vector2)transform.position + direction);

        // Check if the space is free
        Collider2D hit = Physics2D.OverlapPoint(newPos, obstacleLayer);
        if (hit != null)
        {
            // There is an obstacle in the way
            return false;
        }
        AudioManager.Singleton?.PlaySFX("BoxMove");
        // Start moving the block
        StartCoroutine(MoveBlock(newPos));
        return true;
    }

    private IEnumerator MoveBlock(Vector2 destination)
    {
        isMoving = true;

        float elapsedTime = 0f;
        Vector2 startingPos = transform.position;

        while (elapsedTime < moveDuration)
        {
            transform.position = Vector2.Lerp(startingPos, destination, (elapsedTime / moveDuration));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = destination;

        // Snap to grid to correct any minor discrepancies
        SnapToGrid();

        isMoving = false;
    }
}
