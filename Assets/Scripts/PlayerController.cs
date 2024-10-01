using System;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    public Transform keyHolder; // The position where the key will follow the player
    public float floatAmplitude = 0.5f; // Amplitude of the floating effect
    public float floatFrequency = 1f; // Frequency of the floating effect
    public InputHandler inputHandler;

    private GameObject collectedKey;
    private Vector3 keyInitialLocalPosition;
    private float floatTimer;

    private void Start() {
        inputHandler = transform.parent.GetComponent<InputHandler>();
    }

    private void Update()
    {
        if (collectedKey != null)
        {
            // Update the floating effect
            floatTimer += Time.deltaTime * floatFrequency;
            float newY = Mathf.Sin(floatTimer) * floatAmplitude;
            collectedKey.transform.localPosition = keyInitialLocalPosition + new Vector3(0, newY, 0);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Key"))
        {
            // Collect the key and set it to follow the player
            collectedKey = other.gameObject;
            collectedKey.transform.SetParent(transform);
            keyInitialLocalPosition = keyHolder.localPosition;
            collectedKey.transform.localPosition = keyInitialLocalPosition;
        }
    }

    public bool HasKey()
    {
        return collectedKey != null;
    }

    public void ReleaseKey() {
        if (collectedKey != null)
        {
            collectedKey.transform.SetParent(null);
            collectedKey = null;
        }
    }

    public void UseKey() {
        if (collectedKey != null)
        {
            Destroy(collectedKey);
        }
    }
}
