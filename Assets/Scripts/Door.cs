using System;
using UnityEngine;

public enum DoorAction
{
    NextLevel,
    Teleport
}

public class Door : MonoBehaviour
{
    public Sprite closedDoorSprite; // The sprite of the closed door
    public Sprite openDoorSprite; // The sprite of the open door
    public DoorAction doorAction; // The sprite of the open door
    public Vector2Int teleportPosition;
    
    private SpriteRenderer spriteRenderer;
    private bool isOpened;

    void Awake()
    {
        // Get the SpriteRenderer component
        spriteRenderer = GetComponent<SpriteRenderer>();
        
        // Set the initial sprite to the closed door sprite
        spriteRenderer.sprite = closedDoorSprite;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        PlayerController player = other.GetComponent<PlayerController>();
        if (player != null && player.HasKey())
        {
            // Change the sprite to the open door sprite
            spriteRenderer.sprite = openDoorSprite;

            // Optionally, perform additional actions such as unlocking mechanisms
            player.UseKey();
            isOpened = true;
        }
        if(isOpened)
        {
            player.inputHandler.InteractHandler += GetIn;
        }
    }

    private void OnTriggerExit2D(Collider2D other) {
        PlayerController player = other.GetComponent<PlayerController>();
        if (player != null)
        {
            player.inputHandler.InteractHandler -= GetIn;
        }
    }

    private void GetIn(Transform player)
    {
        switch (doorAction)
        {
            case DoorAction.NextLevel:
                GameManager.Singleton.NextLevel();
            break;
            case DoorAction.Teleport:
                player.position = new Vector3(teleportPosition.x,teleportPosition.y);
            break;
        }
    }
}
