using Cinemachine;
using UnityEngine;
using System.Collections;
using System;
using DG.Tweening;

public class MeetShadowAnimation : MonoBehaviour {
    
    public GameObject shadow;
    public float typingSpeed = 0.05f;     // Speed for typing letters
    public float waitTimeBetweenLines = 0.05f; // Time to wait between dialogues

    private HUDController hudController;
    private CinemachineVirtualCamera followingCamera;
    private InputHandler inputHandler;
    private bool isSkipping = false;
    private Transform player;

    string[] shadowDialogue = {
        "Finally... you're here.",
        "The light was too much, I was fading...",
        "I am but a shadow, and the light... it destroys me.",
        "Thank you for giving me a place to rest.",
        "Can I come with you? I know a way out of this cursed tower.",
        "I may be weak, but I can help you escape, if you’ll have me."
    };

    private void Start() {
        hudController = FindAnyObjectByType<HUDController>();
        followingCamera = FindAnyObjectByType<CinemachineVirtualCamera>();
        inputHandler = FindAnyObjectByType<InputHandler>();
    }
    
    private void OnTriggerEnter2D(Collider2D other) {
        if(GameManager.Singleton.unlockShadowPower)
            return;
        inputHandler.inputLock = true;
        AudioManager.Singleton?.PlayMusic("ShadowMeet", 1.5f);
        hudController.SetStoryMode(ShadowMeeting);
    }

    void ShadowMeeting()
    {
        shadow.SetActive(true);
        player = followingCamera.Follow;
        followingCamera.Follow = shadow.transform;
        StartCoroutine(DisplayDialogue(shadowDialogue));
    }

    private IEnumerator DisplayDialogue(string[] dialogueLines)
    {
        for (int i = 0; i < dialogueLines.Length; i++)
        {
            yield return StartCoroutine(TypeAndDisplayLine(dialogueLines[i]));

            // Wait for 0.5 seconds after the full sentence is displayed
            yield return new WaitForSeconds(waitTimeBetweenLines);
        }

        // End of dialogue sequence
        hudController.SetGameMode(()=>{
            inputHandler.inputLock = false;
            shadow.GetComponent<SpriteRenderer>().DOFade(0,0.5f)
            .OnComplete(()=>followingCamera.Follow = player);
            GameManager.Singleton.UnlockShadowPower();
        });
    }

    private IEnumerator TypeAndDisplayLine(string sentence)
    {
        isSkipping = false;

        // Start typing the sentence using the HUDController's method
        Coroutine typingCoroutine = StartCoroutine(hudController.TypeDialogueText(sentence, typingSpeed));

        while (!isSkipping)
        {
            if (Input.anyKeyDown)
            {
                isSkipping = true;
                StopCoroutine(typingCoroutine);
                hudController.dialogueText.text = sentence;  // Instantly display the full sentence
            }
            yield return null;
        }

        if(isSkipping)
        {
            while(!Input.anyKeyDown)
            {
                yield return null;
            }
        }
    }
}