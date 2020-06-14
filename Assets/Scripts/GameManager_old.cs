using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager_old : MonoBehaviour
{
    private GameObject _player;
    private Objective _objective;
    private void Awake()
    {
        _player = GameObject.FindGameObjectWithTag("Player");
        _objective = FindObjectOfType<Objective>();
    }

    void Start()
    {
        
    }

    void Update()
    {
        if (_objective.IsComplete)
        {
            EndLevel();
        }   
    }

    private void EndLevel()
    {
        if (_player != null)
        {
            LoadNextLevel();
        }
    }
    private void LoadNextLevel()
    {
        int totalSceneCount = SceneManager.sceneCountInBuildSettings;
        LoadLevel((SceneManager.GetActiveScene().buildIndex + 1) % totalSceneCount); ;
    }
    private void LoadLevel(int levelIndex)
    {
        if (levelIndex >= 0 && levelIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(levelIndex);
        }
        else
        {
            Debug.LogWarning("GameManager LoadLevel Error: invalid scene specified!");
        }
    }
}
