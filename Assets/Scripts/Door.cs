using UnityEngine;

public class Door : MonoBehaviour
{
    public Sprite closedDoorSprite; // The sprite of the closed door
    public Sprite openDoorSprite; // The sprite of the open door
    
    private SpriteRenderer spriteRenderer;

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
        }
    }
}
