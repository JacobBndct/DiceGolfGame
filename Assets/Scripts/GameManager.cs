using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance {get; private set;}
    
    public static bool CanAdvance = true;
    
    private AudioSource audioSource;

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
        
        // Advance to next scene if we are in the first scene
        if (SceneManager.GetActiveScene().buildIndex == 0)
        {
            NextScene();
        }
        
        // Play music
        audioSource.Play();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
            Restart();
        
        if (Input.GetKeyDown(KeyCode.Space) && CanAdvance)
        {
            NextScene();
            CanAdvance = false;
        }
    }

    private void Restart()
    {
        // Restarts the current Scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void NextScene()
    {
        int currentBuildIndex = SceneManager.GetActiveScene().buildIndex;
        if (currentBuildIndex < SceneManager.sceneCountInBuildSettings - 1)
            SceneManager.LoadScene(currentBuildIndex + 1);
    }

    public void Win()
    {
        // Player cleared the level, set flag to allow advancing to next scene
        CanAdvance = true;        
    }
}
