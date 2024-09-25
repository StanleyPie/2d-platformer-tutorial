using System.Collections;
using System.Collections.Generic;
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
    void Start()
    {
        progressAmount = 0;
        progressSlider.value = 0;
        GemScript.onGemCollect += IncreaseProgressAmount;

        HoldToLoadLevel.onHoldComplete += LoadNextScene;
        this.LoadCanvas.SetActive(false);
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

    void LoadNextScene()
    {
        int nextLevelIndex = (currentLevel == levels.Count - 1) ? 0 : currentLevel + 1;
        this.LoadCanvas.SetActive(false);

        levels[currentLevel].gameObject.SetActive(false);
        levels[nextLevelIndex].gameObject.SetActive(true);

        player.transform.position = Vector3.zero;

        currentLevel = nextLevelIndex;
        progressAmount = 0;
        progressSlider.value = 0;
    }
}
