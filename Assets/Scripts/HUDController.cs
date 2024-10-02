using System;
using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class HUDController : MonoBehaviour
{
    public GameObject hpBar;
    public RectTransform upperFrame;
    public TextMeshProUGUI interactText;
    public TextMeshProUGUI dialogueText;

    private void Start() {
    }

    public void SetInteractText(string text)
    {
        interactText.SetText(text);
    }

    public void SetDialogueText(string text)
    {
        StopAllCoroutines();

        StartCoroutine(TypeDialogueText(text, 0.1f));
    }

    public void SetStoryMode(Action callback = null)
    {
        interactText.gameObject.SetActive(false);
        dialogueText.gameObject.SetActive(true);
        Vector2 currentOffsetMin = upperFrame.offsetMin;
        DOTween.To(() => upperFrame.offsetMin, 
                   x => upperFrame.offsetMin = x, 
                   new Vector2(currentOffsetMin.x, 0), 
                   1.5f);
        hpBar.GetComponent<RectTransform>().DOAnchorPosX(-220,1.4f)
        .OnComplete(()=>callback?.Invoke());
    }

    public void SetGameMode(Action callback = null)
    {
        interactText.gameObject.SetActive(true);
        dialogueText.gameObject.SetActive(false);
        Vector2 currentOffsetMin = upperFrame.offsetMin;
        DOTween.To(() => upperFrame.offsetMin, 
                   x => upperFrame.offsetMin = x, 
                   new Vector2(currentOffsetMin.x, 80), 
                   1.5f);
        hpBar.GetComponent<RectTransform>().DOAnchorPosX(0,1.5f)
        .OnComplete(()=>callback?.Invoke());
    }

    // The coroutine that types the text one character at a time
    public IEnumerator TypeDialogueText(string text, float typingSpeed)
    {
        dialogueText.text = "";  // Clear the current text

        foreach (char letter in text.ToCharArray())
        {
            dialogueText.text += letter;  // Add each letter one by one
            yield return new WaitForSeconds(typingSpeed);  // Wait before showing the next letter
        }
    }
}
