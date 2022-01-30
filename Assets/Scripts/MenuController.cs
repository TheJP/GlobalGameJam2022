using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class MenuController : MonoBehaviour
{
    public GameObject mainMenu;
    public GameObject creditsMenu;
    public GameObject pauseMenu;
    public GameObject gameOverMenu;
    private bool inMainMenu = true;
    private bool isGameOver = false;
    private bool gameIsPaused = false;

    public Button mainMenuPlayButton;
    public Button creditsBackButton;
    public Button pauseMenuContinueButton;
    public Button gameOverMenuRestartButton;
    public TMP_Text score;

    private float startTime;

    public void MainMenu() => SceneManager.LoadScene(0);
    public void PlayGame() {
        RestartDetector.didRestart = false;
        SceneManager.LoadScene(1);
    }

    public void RestartGame() {
        RestartDetector.didRestart = true;
        SceneManager.LoadScene(1);
    }

    public void Start()
    {
        Time.timeScale = 1;
        Cursor.lockState = CursorLockMode.Confined;
        if (SceneManager.GetActiveScene().name == "MainMenu") Cursor.visible = true;
        else Cursor.visible = false;

        
        startTime = Time.time;
        var blobCollision = FindObjectOfType<BlobCollision>();
        if (blobCollision != null)
        {
            blobCollision.BlobDied += GameOver;
        }

        var busCollision = FindObjectOfType<Player3D>();
        if (busCollision != null)
        {
            busCollision.BusDied += GameOver;
        }
    }

    public void TogglePause()
    {
        switch (gameIsPaused)
        {
            case true:
            Time.timeScale = 1;
            Cursor.visible = false;
            gameIsPaused = false;
            pauseMenu.SetActive(false);
            break;

            case false:
            Time.timeScale = 0;
            Cursor.visible = true;
            gameIsPaused = true;
            pauseMenu.SetActive(true);
            pauseMenuContinueButton.Select();
            break;
        }

    }


    public void ToggleCredits()
    {
        switch(inMainMenu) 
        {
            case true:
            mainMenu.SetActive(false);
            creditsMenu.SetActive(true);
            inMainMenu = false;
            creditsBackButton.Select();
            break;
            
            case false: 
            mainMenu.SetActive(true);
            creditsMenu.SetActive(false);
            inMainMenu = true;
            mainMenuPlayButton.Select();
            break;
        }
    }


    public void QuitGame()
    {
        Application.Quit();
    }


    public void MenuAction()
    {
        if (!isGameOver)
        {
            TogglePause();
        }
    }

    private void GameOver()
    {
        isGameOver = true;
        Cursor.visible = true;
        score.text = $"{Time.time - startTime:0.0}s";
        gameOverMenu.SetActive(true);
        Time.timeScale = 0;
        gameOverMenuRestartButton.Select();
    }
}
