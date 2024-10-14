using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameCtrl : MonoBehaviour
{
    int progressAmount;
    public Slider progressSlider;

    public GameObject player;
    public GameObject LoadCanvas;
    public List<GameObject> levels;
    int currentLevel = 0;

    public GameObject gameOverScreen;
    public TMP_Text survivedText;
    int survivedLevelCount;

    public static event Action OnReset;
    void Start()
    {
        progressAmount = 0;
        progressSlider.value = 0;
        GemScript.onGemCollect += IncreaseProgressAmount;
        HoldToLoadLevel.onHoldComplete += LoadNextLevel;
        PlayerHealth.OnPlayerDied += GameOverScreen;
        this.LoadCanvas.SetActive(false);
        gameOverScreen.SetActive(false);
    }

    public void ResetGame()
    {
        this.gameOverScreen.SetActive(false);
        MusicManager.PlayBackgroundMusic(false);
        this.survivedLevelCount = 0;
        this.LoadLevel(0, false);
        OnReset.Invoke();
        Time.timeScale = 1;
    }

    void GameOverScreen()
    {
        this.gameOverScreen.SetActive(true);
        MusicManager.PauseBGMusic();
        this.survivedText.text = "YOU SURVIVED " + survivedLevelCount + " lEVEL";
        if (survivedLevelCount != 1) survivedText.text += "S";
        Time.timeScale = 0;
    }

    void IncreaseProgressAmount(int amount)
    {
        progressAmount += amount;
        progressSlider.value = progressAmount;  
        if (progressAmount >= 100)
        { 
            LoadCanvas.SetActive(true);

        }
    }

    void LoadLevel(int level, bool wantSurvivedIncrease)
    {
        this.LoadCanvas.SetActive(false);

        levels[currentLevel].gameObject.SetActive(false);
        levels[level].gameObject.SetActive(true);

        player.transform.position = Vector3.zero;

        currentLevel = level;
        progressAmount = 0;
        progressSlider.value = 0;
        if(wantSurvivedIncrease) survivedLevelCount++;
    }

    void LoadNextLevel()
    {
        int nextLevelIndex = (currentLevel == levels.Count - 1) ? 0 : currentLevel + 1;
        this.LoadLevel(nextLevelIndex, true);
    }
}
