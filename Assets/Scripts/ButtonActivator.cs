using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ButtonActivator : MonoBehaviour
{
    public GameObject[] hiddenPathBlocks; // Array of hidden path blocks
    public string playerTag = "Player"; // The tag used to identify the player
    public string[] extraTriggerTags; // The tag used to identify the player
    public float activationDelay = 0.5f; // Delay between each block activation
    public Sprite normalSprite; // The normal sprite of the button
    public Sprite activeSprite; // The active sprite of the button
    public bool holdMode;
    public bool freezeBlock;

    private Coroutine activationCoroutine;
    private SpriteRenderer spriteRenderer;
    private bool isActive;
    private HashSet<Collider2D> onStayColliderSet;

    private void Awake()
    {
        onStayColliderSet = new HashSet<Collider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        // Set the initial sprite to the normal sprite
        spriteRenderer.sprite = normalSprite;
        isActive = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(playerTag) || CheckExtraTags(other.tag))
        {
            // Change the button sprite to active
            spriteRenderer.sprite = activeSprite;
            Debug.Log("" + other.gameObject.name);
            onStayColliderSet.Add(other);
            // Start the block activation coroutine when the player enters the trigger collider
            if(!isActive && activationCoroutine != null){
                StopCoroutine(activationCoroutine);
                activationCoroutine = null;
            }
            if (activationCoroutine == null)
            {
                isActive = true;
                activationCoroutine = StartCoroutine(ActivateBlocksOneByOne());
            }
            if(freezeBlock && other.CompareTag("Pushable") && other.TryGetComponent(out Rigidbody2D rigidbody2D))
            {
                rigidbody2D.bodyType = RigidbodyType2D.Kinematic;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag(playerTag) || CheckExtraTags(other.tag))
        {
            // Revert the button sprite to normal
            if(onStayColliderSet.Contains(other)) onStayColliderSet.Remove(other);
            if(holdMode)
            {
                isActive = onStayColliderSet.Count > 0;
                if(!isActive)
                {
                    if(activationCoroutine != null)StopCoroutine(activationCoroutine);
                    activationCoroutine = StartCoroutine(ActivateBlocksOneByOne());
                    spriteRenderer.sprite = normalSprite;
                }
            }
            else
            {
                spriteRenderer.sprite = normalSprite;
            }
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

    private bool CheckExtraTags(string tag)
    {
        foreach(string extraTag in extraTriggerTags)
        {
            if(tag.Equals(extraTag))
            {
                return true;
            }
        }

        return false;
    }
}
