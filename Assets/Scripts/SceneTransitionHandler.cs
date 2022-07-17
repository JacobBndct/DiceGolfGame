using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransitionHandler : MonoBehaviour
{
    [SerializeField] private RectTransform transitionRect;
    
    [SerializeField] private float transitionTime = 1f;
    
    private GameManager gameManager;

    private void Start()
    {
        gameManager = GetComponent<GameManager>();
    }

    private void AnimateIn()
    {
        LeanTween.alpha(transitionRect, 1, 0);
        transitionRect.gameObject.SetActive(true);
        
        // Transition in alpha
        LeanTween.alpha(transitionRect, 0, transitionTime).setOnComplete(() =>
        {
            transitionRect.gameObject.SetActive(false);
        });
    }
    
    public void NextScene()
    {
        int currentBuildIndex = SceneManager.GetActiveScene().buildIndex;
        if (currentBuildIndex < SceneManager.sceneCountInBuildSettings - 1)
        {
            SceneManager.LoadScene(currentBuildIndex + 1);
            AnimateIn();
            gameManager.UpdateHoleText();
        }
        
    }
    
    public void Restart()
    {
        // Restarts the current Scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void TransitionToNextScene()
    {
        LeanTween.alpha(transitionRect, 0, 0);
        transitionRect.gameObject.SetActive(true);
        
        // Transition out alpha and go to next scene
        LeanTween.alpha(transitionRect, 1, transitionTime).setOnComplete(NextScene);
    }
}
