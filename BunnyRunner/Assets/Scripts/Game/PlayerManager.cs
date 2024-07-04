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
    public Text[] inGameLootText;
    public Text finalScoreLootText;
    public List<Button> DoubleCarrotButtons;
    public List<Text> totalScoreTextElements;
    AdsManager adManager;
    public AudioSource bgMusicSource;
    public AudioClip bgMusicClip;

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
        UpdateTotalScoreUI();

        if (bgMusicClip != null)
        {
            bgMusicSource = gameObject.AddComponent<AudioSource>();
            bgMusicSource.clip = bgMusicClip;
            bgMusicSource.loop = true;
            bgMusicSource.Play();
        }

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
        scoreText.text = currentEarnedScore.ToString();
        UpdateLootTexts();

        if (gameOver)
        {
            Time.timeScale = 0;
            if (currentEarnedScore > PlayerPrefs.GetInt("HighScore", 0))
            {
                newRecordPanel.SetActive(true);
                PlayerPrefs.SetInt("HighScore", currentEarnedScore);
            }
            gameOverPanel.SetActive(true);
            SaveScore();
            LogAllSavedScores();
            Destroy(gameObject);
        }

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
            foreach (Text text in inGameLootText)
            {
                if (text != null)
                {
                    text.text = currentEarnedScore.ToString();
                }
            }
        }

        if (finalScoreLootText != null)
        {
            finalScoreLootText.text = currentEarnedScore.ToString();
        }
    }

    public void ShowWinnerPopup()
    {
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

    public void DoubleEarnedScore()
    {
        int doubledScore = currentEarnedScore * 2;
        totalScore += doubledScore;
        currentEarnedScore = doubledScore;
        UpdateScoreUI();
        UpdateTotalScoreUI();
        Debug.Log("Doubled Earned Score: " + doubledScore);
        SaveScore();
    }

    private void SaveScore()
    {
        PlayerPrefs.SetInt("TotalScore", totalScore);
        PlayerPrefs.SetInt("CurrentEarnedScore", currentEarnedScore);
        PlayerPrefs.Save();
    }

    private void LoadScore()
    {
        totalScore = PlayerPrefs.GetInt("TotalScore", 0);
        currentEarnedScore = PlayerPrefs.GetInt("CurrentEarnedScore", 0);
    }

    private void LogAllSavedScores()
    {
        int savedTotalScore = PlayerPrefs.GetInt("TotalScore", 0);
        int savedCurrentEarnedScore = PlayerPrefs.GetInt("CurrentEarnedScore", 0);
        Debug.Log($"Saved Total Score: {savedTotalScore}");
        Debug.Log($"Saved Current Earned Score: {savedCurrentEarnedScore}");
    }

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
        // Reset isGameStarted to false when going to MainMenu
        isGameStarted = false;
        SceneManager.LoadScene("MainMenu");
    }

    private void OnDestroy()
    {
        if (bgMusicSource != null)
        {
            bgMusicSource.Stop();
        }
    }
}
