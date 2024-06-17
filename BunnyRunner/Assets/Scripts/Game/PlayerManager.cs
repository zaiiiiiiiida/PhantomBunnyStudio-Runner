using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class PlayerManager : MonoBehaviour
{
    public static bool gameOver;
    public GameObject gameOverPanel;
    [SerializeField] private Transform target;
    public GameObject winnerPanel;
    public static bool isGameStarted;
    public GameObject startingText;
    public GameObject newRecordPanel;

    private Vector3 targetPosition;
    private static int score;
    public Text scoreText;

    // References to loot text UI elements
    public Text inGameLootText;
    public Text finalScoreLootText;

    private void Awake()
    {
        targetPosition = target.position;
    }

    void Start()
    {
        score = 0;
        Time.timeScale = 1;
    }

    void Update()
    {
        // Update UI
        scoreText.text = score.ToString();
        UpdateLootTexts();

        // Game Over
        if (gameOver)
        {
            Time.timeScale = 0;
            if (score > PlayerPrefs.GetInt("HighScore", 0))
            {
                newRecordPanel.SetActive(true);
                PlayerPrefs.SetInt("HighScore", score);
            }
            gameOverPanel.SetActive(true);
            Destroy(gameObject);
        }

        // Start Game
        if (SwipeManager.tap && !isGameStarted)
        {
            isGameStarted = true;
            Destroy(startingText);
        }
    }

    public void AddScore(int amount)
    {
        score += amount;
        UpdateLootTexts();
    }

    private void UpdateScoreUI()
    {
        scoreText.text = score.ToString();
    }

    private void UpdateLootTexts()
    {
        if (inGameLootText != null)
        {
            inGameLootText.text = score.ToString();
        }
        if (finalScoreLootText != null)
        {
            finalScoreLootText.text = score.ToString();
        }
    }

    public void ShowWinnerPopup()
    {
        // Set the winner panel active
        if (winnerPanel != null)
        {
            winnerPanel.SetActive(true);
        }
    }
}
