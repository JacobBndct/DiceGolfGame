using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance {get; private set;}
    
    public static bool CanAdvance;
    
    private AudioSource audioSource;
    private SceneTransitionHandler sceneTransitionHandler;

    private int currentRollCount = 0;
    
    private List<int> scoreList = new List<int>();

    [SerializeField] private TextMeshProUGUI holeText;
    [SerializeField] private TextMeshProUGUI rollsText;
    [SerializeField] private TextMeshProUGUI scoreText;

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
        if (Input.GetKeyDown(KeyCode.R) && !CanAdvance)
            sceneTransitionHandler.Restart();
        
        if (Input.GetKeyDown(KeyCode.Space) && CanAdvance)
        {
            sceneTransitionHandler.TransitionToNextScene();
            // Record score
            currentRollCount = 0;
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

    public void IncreaseRolls()
    {
        
        currentRollCount++;
        UpdateRollsText();
    }

    public void DisableRollsText()
    {
        rollsText.gameObject.SetActive(false);
    }

    public void ResetRollCount()
    {
        currentRollCount = 0;
        UpdateRollsText();
    }
    
    public void UpdateRollsText()
    {
        var buildIndex = SceneManager.GetActiveScene().buildIndex;

        if (buildIndex < 2)
        {
            rollsText.gameObject.SetActive(false);
            return;
        }
        rollsText.gameObject.SetActive(true);
        
        rollsText.text = currentRollCount + " Rolls";
    }
    
    private string GetScoreString()
    {
        var scoreString = "";
        for (int i = 0; i < scoreList.Count; i++)
        {
            scoreString += "Hole " + (i+1) + ":\t" + scoreList[i] + " Rolls";
            if (i < scoreList.Count - 1)
                scoreString += "\n";
        }
        return scoreString;
    }
    
    public void ShowScore()
    {
        var scoreString = GetScoreString();
        scoreText.text = scoreString;
        scoreText.gameObject.SetActive(true);
    }

    public void Win()
    {
        // Player cleared the level, set flag to allow advancing to next scene
        if (SceneManager.GetActiveScene().buildIndex > 1)
        {
            scoreList.Add(currentRollCount);
        }
        
        CanAdvance = true;        
    }
}
