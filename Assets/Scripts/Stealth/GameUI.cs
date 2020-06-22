using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameUI : MonoBehaviour
{
    public GameObject gameloseUI;
    public GameObject gameWinUI;
    bool gameIsOver;
    private void Start()
    {
        Guard.OnGuardHasSpottedPlayer += ShowGameLoseUI;
        FindObjectOfType<Player>().OnReachedEndOfLevel += ShowGameWinUI;
    }
    private void Update()
    {
        if (gameIsOver)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                SceneManager.LoadScene("Stealth");
            }            
        }
    }
    void ShowGameWinUI()
    {
        OnGameOver(gameWinUI);
    }
    void ShowGameLoseUI()
    {
        OnGameOver(gameloseUI);
    }

    void OnGameOver(GameObject gameOverUI)
    {
        gameOverUI.SetActive(true);
        gameIsOver = true;
        Guard.OnGuardHasSpottedPlayer -= ShowGameLoseUI;
        FindObjectOfType<Player>().OnReachedEndOfLevel -= ShowGameWinUI;
    }
}
