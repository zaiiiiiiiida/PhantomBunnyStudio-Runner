using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.SceneManagement;

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
    private static int totalScore;
    private int currentEarnedScore;
    public Text scoreText;
    public Text inGameLootText;
    public Text finalScoreLootText;
    public List<Button> DoubleCarrotButtons; // Changed to List<Button>
    public List<Text> totalScoreTextElements;
    AdsManager adManager;
    // Background music variables
    public AudioSource bgMusicSource; // Reference to the AudioSource component for background music
    public AudioClip bgMusicClip; // Background music audio clip

    private void Awake()
    {
        targetPosition = target.position;
        adManager = FindAnyObjectByType<AdsManager>();
    }

    void Start()
    {
        totalScore = PlayerPrefs.GetInt("TotalScore", 0);
        currentEarnedScore = 0;
        Time.timeScale = 1;
        UpdateTotalScoreUI(); // Update the UI with the loaded total score

        // Initialize background music
        if (bgMusicClip != null)
        {
            bgMusicSource = gameObject.AddComponent<AudioSource>(); // Add AudioSource if not already present
            bgMusicSource.clip = bgMusicClip;
            bgMusicSource.loop = true; // Loop the background music
            bgMusicSource.Play(); // Start playing background music
        }

        // Add event listeners to all DoubleCarrotButtons
        foreach (Button button in DoubleCarrotButtons)
        {
            if (button != null)
            {
                button.onClick.AddListener(adManager.ShowRewardedAdDoubleScore);
            }
        }
    }

    void Update()
    {
        // Update UI
        scoreText.text = currentEarnedScore.ToString();
        UpdateLootTexts();

        // Game Over
        if (gameOver)
        {
            Time.timeScale = 0;
            if (currentEarnedScore > PlayerPrefs.GetInt("HighScore", 0))
            {
                newRecordPanel.SetActive(true);
                PlayerPrefs.SetInt("HighScore", currentEarnedScore);
            }
            gameOverPanel.SetActive(true);
            SaveScore(); // Save scores when the game is over
            LogAllSavedScores(); // Log all saved scores
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
        currentEarnedScore += amount;
        UpdateScoreUI();
        UpdateLootTexts();
    }

    private void UpdateScoreUI()
    {
        scoreText.text = currentEarnedScore.ToString();
    }

    private void UpdateLootTexts()
    {
        if (inGameLootText != null)
        {
            inGameLootText.text = currentEarnedScore.ToString();
        }
        if (finalScoreLootText != null)
        {
            finalScoreLootText.text = currentEarnedScore.ToString();
        }
    }

    public void ShowWinnerPopup()
    {
        // Set the winner panel active
        if (winnerPanel != null)
        {
            winnerPanel.SetActive(true);
            UnlockNewLevel();
        }
    }

    void UnlockNewLevel()
    {
        if (SceneManager.GetActiveScene().buildIndex >= PlayerPrefs.GetInt("ReachedIndex"))
        {
            PlayerPrefs.SetInt("ReachedIndex", SceneManager.GetActiveScene().buildIndex + 1);
            PlayerPrefs.SetInt("UnlockedLevel", PlayerPrefs.GetInt("UnlockedLevel", 1));
            PlayerPrefs.Save();
        }
    }

    // Public function to calculate earned score and double it
    public void DoubleEarnedScore()
    {
        int doubledScore = currentEarnedScore * 2;
        totalScore += doubledScore; // Add the doubled score to the total score
        currentEarnedScore = doubledScore; // Update the current earned score to show it doubled
        UpdateScoreUI(); // Update the UI with the new current earned score
        UpdateTotalScoreUI(); // Update the UI with the new total score
        Debug.Log("Doubled Earned Score: " + doubledScore); // Log for demonstration
        SaveScore(); // Save the updated scores after doubling
    }

    // Save scores to PlayerPrefs
    private void SaveScore()
    {
        PlayerPrefs.SetInt("TotalScore", totalScore);
        PlayerPrefs.SetInt("CurrentEarnedScore", currentEarnedScore);
        PlayerPrefs.Save(); // Save the PlayerPrefs to disk
    }

    // Load scores from PlayerPrefs
    private void LoadScore()
    {
        totalScore = PlayerPrefs.GetInt("TotalScore", 0);
        currentEarnedScore = PlayerPrefs.GetInt("CurrentEarnedScore", 0);
    }

    // Log all saved scores to console
    private void LogAllSavedScores()
    {
        int savedTotalScore = PlayerPrefs.GetInt("TotalScore", 0);
        int savedCurrentEarnedScore = PlayerPrefs.GetInt("CurrentEarnedScore", 0);
        Debug.Log($"Saved Total Score: {savedTotalScore}");
        Debug.Log($"Saved Current Earned Score: {savedCurrentEarnedScore}");
    }

    // Update all Text elements in the totalScoreTextElements list with the total score
    private void UpdateTotalScoreUI()
    {
        foreach (Text textElement in totalScoreTextElements)
        {
            if (textElement != null)
            {
                textElement.text = totalScore.ToString();
            }
        }
    }

    public void GoToMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    // Stop background music when the game is over
    private void OnDestroy()
    {
        if (bgMusicSource != null)
        {
            bgMusicSource.Stop();
        }
    }
}
