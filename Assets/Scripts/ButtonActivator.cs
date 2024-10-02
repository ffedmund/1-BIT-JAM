using UnityEngine;
using System.Collections;

public class ButtonActivator : MonoBehaviour
{
    public GameObject[] hiddenPathBlocks; // Array of hidden path blocks
    public string playerTag = "Player"; // The tag used to identify the player
    public float activationDelay = 0.5f; // Delay between each block activation
    public Sprite normalSprite; // The normal sprite of the button
    public Sprite activeSprite; // The active sprite of the button
    public bool switchMode;

    private Coroutine activationCoroutine;
    private SpriteRenderer spriteRenderer;
    private bool isActive;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        // Set the initial sprite to the normal sprite
        spriteRenderer.sprite = normalSprite;
        isActive = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(playerTag))
        {
            // Change the button sprite to active
            spriteRenderer.sprite = activeSprite;
            // Start the block activation coroutine when the player enters the trigger collider
            if (activationCoroutine == null)
            {
                isActive = switchMode? !isActive : true;
                activationCoroutine = StartCoroutine(ActivateBlocksOneByOne());
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag(playerTag))
        {
            // Revert the button sprite to normal
            spriteRenderer.sprite = normalSprite;
        }
    }

    private IEnumerator ActivateBlocksOneByOne()
    {
        foreach (GameObject block in hiddenPathBlocks)
        {
            block.SetActive(isActive);
            yield return new WaitForSeconds(activationDelay);
        }
        activationCoroutine = null;
    }
}
