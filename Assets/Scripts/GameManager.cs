using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance {get; private set;}
    
    public static bool CanAdvance = true;
    
    private AudioSource audioSource;
    private SceneTransitionHandler sceneTransitionHandler;

    [SerializeField] private TextMeshProUGUI holeText;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        sceneTransitionHandler = GetComponent<SceneTransitionHandler>();
        
        // Advance to next scene if we are in the first scene
        if (SceneManager.GetActiveScene().buildIndex == 0)
        {
            sceneTransitionHandler.NextScene();
        }
        
        // Play music
        audioSource.Play();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
            sceneTransitionHandler.Restart();
        
        if (Input.GetKeyDown(KeyCode.Space) && CanAdvance)
        {
            sceneTransitionHandler.TransitionToNextScene();
            CanAdvance = false;
        }
    }
    
    public void UpdateHoleText()
    {
        // Build index is always off by one but I don't care this works enough
        var buildIndex = SceneManager.GetActiveScene().buildIndex;

        if (buildIndex < 1 || buildIndex == SceneManager.sceneCountInBuildSettings-2)
        {
            holeText.gameObject.SetActive(false);
            return;
        }
        
        holeText.gameObject.SetActive(true);
        
        holeText.text = "Hole " + (buildIndex);
    }

    public void Win()
    {
        // Player cleared the level, set flag to allow advancing to next scene
        CanAdvance = true;        
    }
}
