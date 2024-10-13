using UnityEngine;
using DG.Tweening;
using Unity.VisualScripting;  // Import DoTween namespace

public class BossSpawner : MonoBehaviour
{
    public GameObject boss;                  // Reference to the boss GameObject
    public string bossName;
    public Transform bossPivotPoint;         // The point where we want the boss to move
    public CinemachineCameraShaker cameraShaker;
    public float bossMoveDuration = 2f;      // Duration to move the boss to the pivot
    public float cameraShakeDuration = 1f;   // Duration of the camera shake
    public float cameraShakeStrength = 0.5f; // Strength of the camera shake
    public int cameraShakeVibrato = 10;      // Vibrato (how much the shake oscillates)
    public float cameraShakeRandomness = 90f;// Randomness of camera shake

    public Transform mainCamera;               // Reference to the main camera
    private bool spawned = false;

    private void Start()
    {
        if (boss != null && bossPivotPoint != null)
        {
            // Make sure the boss is inactive before spawning
            boss.SetActive(false);
        }
    }

    // Public method to spawn and move the boss
    public void SpawnBoss()
    {
        if (boss == null || bossPivotPoint == null || mainCamera == null)
        {
            Debug.LogWarning("BossSpawner setup is incomplete. Please assign the boss, pivot, and camera.");
            return;
        }
        spawned = true;
        // Activate the boss and move it
        boss.SetActive(true);
        AudioManager.Singleton.PlayMusic("Boss");

        // Start moving the boss to the pivot position
        MoveBossToPivot();
    }

    // Method that moves the boss to its pivot and shakes the camera during the movement
    private void MoveBossToPivot()
    {
        // Start the boss's movement to the designated position with DoTween
        boss.transform.DOMove(bossPivotPoint.position, bossMoveDuration)
            .OnStart(() =>
            {
                // Start shaking the camera when boss movement starts
                cameraShaker.ShakeCamera();
            })
            .OnComplete(() =>
            {
                // Optional: Add more logic for when the boss reaches its pivot (e.g. start the fight)
                Debug.Log("Boss has reached the correct position.");
                BossController bossController = boss.GetComponentInChildren<BossController>();
                BossHPBar bossHPBar = FindAnyObjectByType<BossHPBar>(FindObjectsInactive.Include);
                bossHPBar.gameObject.SetActive(true);
                bossController.OnHurt += bossHPBar.SetUp(bossController.hp,bossName);
                bossController.SpawnBoss();
            });
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if(other.CompareTag("Player") && !spawned)
        {
            SpawnBoss();
        }
    }
}
