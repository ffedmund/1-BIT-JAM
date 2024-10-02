using UnityEngine;
using TMPro;
using DG.Tweening;

public class PressStartAnimation : MonoBehaviour
{
    [Header("Text Settings")]
    public TextMeshProUGUI pressStartText;

    [Header("Fade Animation Settings")]
    public bool useFadeAnimation = true;
    public float fadeDuration = 1f;

    [Header("Scale Animation Settings")]
    public bool useScaleAnimation = false;
    public float scaleDuration = 0.5f;
    public float startScale = 1f;
    public float endScale = 1.1f;

    void Start()
    {
        if (pressStartText == null)
        {
            pressStartText = GetComponent<TextMeshProUGUI>();
        }

        if (useFadeAnimation)
        {
            Invoke("StartFadeAnimation",0.1f);
        }

        if (useScaleAnimation)
        {
            Invoke("StartScaleAnimation",0.1f);
        }
    }

    void StartFadeAnimation()
    {
        // Ensure the text is fully visible at the start
        pressStartText.alpha = 1f;

        // Create a looping fade animation
        pressStartText.DOFade(0f, fadeDuration)
            .SetLoops(-1, LoopType.Yoyo)
            .SetEase(Ease.InOutSine);
    }

    void StartScaleAnimation()
    {
        // Set initial scale
        pressStartText.transform.localScale = Vector3.one * startScale;

        // Create a looping scale animation
        pressStartText.transform.DOScale(endScale, scaleDuration)
            .SetLoops(-1, LoopType.Yoyo)
            .SetEase(Ease.InOutSine);
    }
}
