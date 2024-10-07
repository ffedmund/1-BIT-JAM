using System;
using UnityEngine;

public enum DoorAction
{
    NextLevel,
    Teleport
}

[RequireComponent(typeof(InteractNotice))]
public class Door : MonoBehaviour
{
    public Sprite closedDoorSprite; // The sprite of the closed door
    public Sprite openDoorSprite; // The sprite of the open door
    public DoorAction doorAction; // The sprite of the open door
    public Vector2 teleportPosition;
    public bool isOpened;
    
    private SpriteRenderer spriteRenderer;
    private InteractNotice interactNotice;

    void Awake()
    {
        // Get the SpriteRenderer component
        spriteRenderer = GetComponent<SpriteRenderer>();
        interactNotice = GetComponent<InteractNotice>();
        
        // Set the initial sprite to the closed door sprite
        spriteRenderer.sprite = isOpened? openDoorSprite : closedDoorSprite;
        UpdateInteractText();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        PlayerController player = other.GetComponent<PlayerController>();
        if (player != null && player.HasKey() && !isOpened)
        {
            // Change the sprite to the open door sprite
            spriteRenderer.sprite = openDoorSprite;

            // Optionally, perform additional actions such as unlocking mechanisms
            player.UseKey();
            isOpened = true;
        }
        if(isOpened && player && player.GetType() != typeof(ShadowPlayerController))
        {
            player.inputHandler.InteractHandler += GetIn;
        }
        UpdateInteractText();
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
        AudioManager.Singleton?.PlaySFX("DoorClose");
        switch (doorAction)
        {
            case DoorAction.NextLevel:
                GameManager.Singleton.NextLevel();
                player.GetComponent<PlayerController>().inputHandler.InteractHandler -= GetIn;
            break;
            case DoorAction.Teleport:
                player.position = new Vector3(teleportPosition.x,teleportPosition.y);
            break;
        }
    }

    private void UpdateInteractText()
    {
        interactNotice.interactText = isOpened? "Press E" : "Locked";
    }
}
