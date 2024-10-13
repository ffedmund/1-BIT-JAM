using UnityEngine;
using DG.Tweening;   // Don't forget to add DoTween namespace for tweening

public class BossHeadMovement : MonoBehaviour
{
    public Transform bossHead;  // The head that will be moved
    public float moveDistance = 2f;  // How far up and down the head moves
    public float moveDuration = 1.5f;  // How long one up/down cycle takes
    public bool isMoving = true;  // Control movement start and stop

    private Vector3 initialPosition;  // To store the original position of the head

    // Starts the up and down movement using DoTween
    public void StartHeadMovement()
    {
        initialPosition = bossHead.position;
        if (isMoving)
        {
            // The sequence will move the head up, then down, and will loop infinitely
            bossHead.DOMoveY(initialPosition.y + moveDistance, moveDuration)
                    .SetEase(Ease.InOutSine)       // Apply smooth easing for natural movement
                    .SetLoops(-1, LoopType.Yoyo)   // Infinite Yoyo loop (up and down)
                    .SetDelay(0.5f);               // Optional pause before starting movement
        }
    }

    // Stops head movement (optional method if you need to stop the movement later)
    public void StopHeadMovement()
    {
        isMoving = false;
        bossHead.DOKill();  // Stops all DoTween animations on the boss head
    }

    // Resets head position to its original place
    public void ResetHeadPosition()
    {
        StopHeadMovement();
        bossHead.position = initialPosition;
    }
}
