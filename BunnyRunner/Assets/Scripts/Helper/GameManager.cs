using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public PlayerController playerController;
    public AIEnemy enemyScript;
    public GameObject winnerPanel;

    private bool isGameOver = false;

    void Start()
    {
        if (enemyScript != null)
        {
            enemyScript.OnEnemyDeath += HandleEnemyDeath;
        }

        if (winnerPanel != null)
        {
            winnerPanel.SetActive(false);
        }
    }

    private void HandleEnemyDeath()
    {
        if (isGameOver)
            return;

        isGameOver = true;

        // Declare the player as the winner
        playerController.enabled = false; // Disable player movement
        playerController.anim.SetTrigger("Dance"); // Set dance animation

        // Show the winner panel
        if (winnerPanel != null)
        {
            winnerPanel.SetActive(true);
        }
    }

    void OnDestroy()
    {
        if (enemyScript != null)
        {
            enemyScript.OnEnemyDeath -= HandleEnemyDeath;
        }
    }
}
