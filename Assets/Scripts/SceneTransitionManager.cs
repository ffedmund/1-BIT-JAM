using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class SceneTransitionManager : MonoBehaviour
{
    public static SceneTransitionManager Singleton;

    [Header("Transition Animator")]
    public Animator transitionAnimator;

    [System.Serializable]
    public class TransitionAnimation
    {
        public string animationStateName;
        public string reverseAnimationStateName;
    }

    [Header("Transition Animations")]
    public List<TransitionAnimation> transitionAnimations;

    private void Awake()
    {
        // Implement Singleton pattern
        if (Singleton == null)
        {
            Singleton = this;
            DontDestroyOnLoad(gameObject); // Persist across scenes
            transitionAnimator.gameObject.SetActive(false);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Call this method to load a new scene with a transition effect.
    /// </summary>
    /// <param name="sceneName">Name of the scene to load.</param>
    public void LoadScene(string sceneName)
    {
        StartCoroutine(LoadSceneCoroutine(sceneName));
    }

    private IEnumerator LoadSceneCoroutine(string sceneName)
    {
        // Randomly select a transition animation
        TransitionAnimation transition = transitionAnimations[Random.Range(0, transitionAnimations.Count)];
        transitionAnimator.gameObject.SetActive(true);

        // Play the transition animation
        transitionAnimator.Play(transition.animationStateName);

        // Begin loading the scene asynchronously
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        asyncLoad.allowSceneActivation = false;

        // Wait for the transition animation to finish
        yield return StartCoroutine(WaitForAnimation(transition.animationStateName));

        // Wait until the scene is fully loaded
        while (asyncLoad.progress < 0.9f)
        {
            yield return null;
        }

        // Allow scene activation
        asyncLoad.allowSceneActivation = true;

        // Wait until the scene has been activated
        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        // Play the reverse transition animation
        transitionAnimator.Play(transition.reverseAnimationStateName);

        // Wait for the reverse animation to finish
        yield return StartCoroutine(WaitForAnimation(transition.reverseAnimationStateName));

        transitionAnimator.gameObject.SetActive(false);
    }

    private IEnumerator WaitForAnimation(string stateName)
    {
        // Wait until the animator is in the specified state
        while (!transitionAnimator.GetCurrentAnimatorStateInfo(0).IsName(stateName))
        {
            yield return null;
        }

        // Wait until the current animation has finished playing
        while (transitionAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f)
        {
            yield return null;
        }
    }
}
