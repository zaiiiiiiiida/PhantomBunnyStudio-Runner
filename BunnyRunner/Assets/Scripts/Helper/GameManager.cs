using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject enemyPrefab; // Reference to the enemy prefab
    public Transform player; // Reference to the player transform
    public float spawnDistance = 10f; // Distance behind the player to spawn the enemy

    void Start()
    {
        // Check if the player reference is set
        if (player == null)
        {
            Debug.LogError("Player reference is not set in the GameManager!");
            return;
        }

        // Check if the enemy prefab reference is set
        if (enemyPrefab == null)
        {
            Debug.LogError("Enemy prefab reference is not set in the GameManager!");
            return;
        }

        // Instantiate the enemy prefab behind the player
        Vector3 spawnPosition = player.position - player.forward * spawnDistance;
        Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
    }
}
