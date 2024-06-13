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

    public static bool isGameStarted;
    public GameObject startingText;
    public GameObject newRecordPanel;
    
   
  

 

    private Vector3 targetPosition;
    private static int score;
    public Text scoreText;
    //public Text gemsText;
    //public Text newRecordText;
    //public static bool isGamePaused;
   // public GameObject[] characterPrefabs;

    private void Awake()
    {
        targetPosition = target.position;
        //int index = PlayerPrefs.GetInt("SelectedCharacter");
     
        //GameObject go = Instantiate(characterPrefabs[index], transform.position, Quaternion.identity);
        //adManager = FindObjectOfType<AdManager>();
    }

 

    void Start()
    {
        score = 0;
        Time.timeScale = 1;
       // gameOver = isGameStarted = isGamePaused = false;

        //adManager.RequestBanner();
        //adManager.RequestInterstitial();
        //adManager.RequestRewardBasedVideo();
    }

    void Update()
    {
        //Update UI
        //gemsText.text = PlayerPrefs.GetInt("TotalGems", 0).ToString();
        scoreText.text = score.ToString();

        //Game Over
        if (gameOver)
        {
            Time.timeScale = 0;
            if (score > PlayerPrefs.GetInt("HighScore", 0))
            {
                newRecordPanel.SetActive(true);
               // newRecordText.text = "New \nRecord\n" + score;
                PlayerPrefs.SetInt("HighScore", score);
            }
            else
            {
                int i = Random.Range(0, 6);
                //if (i == 0)
                //adManager.ShowInterstitial();
                //else if (i == 3)
                //adManager.ShowRewardBasedVideo();
            }

            gameOverPanel.SetActive(true);
            Destroy(gameObject);
        }

        //Start Game
        if (SwipeManager.tap && !isGameStarted)
        {
            isGameStarted = true;
            Destroy(startingText);
        }
    }


    public void AddScore( int amount)
    {
        score += amount;
    }

    private void UpdateScoreUI()
    {
        scoreText.text = score.ToString();
    }
}
