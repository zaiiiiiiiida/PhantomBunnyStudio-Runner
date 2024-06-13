using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileManager : MonoBehaviour
{
    private List<GameObject> activeTiles;
    public GameObject[] tilePrefabs;
    public GameObject particleEffectPrefab; // Reference to particle effect prefab

    public float tileLength = 30;
    public int numberOfTiles = 3;
    public int totalNumOfTiles = 8;

    public float zSpawn = 0;

    private Transform playerTransform;
    private AIEnemy instantiatedEnemy;
    private bool obstaclesProcessed = false; // Flag to ensure obstacles are processed only once

    private int previousIndex;

    void Start()
    {
        activeTiles = new List<GameObject>();
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;

        for (int i = 0; i < numberOfTiles; i++)
        {
            if (i == 0)
                SpawnTile();
            else
                SpawnTile(Random.Range(0, totalNumOfTiles));
        }
    }

    void Update()
    {
        if (playerTransform.position.z - 30 >= zSpawn - (numberOfTiles * tileLength))
        {
            int index = Random.Range(0, totalNumOfTiles);
            while (index == previousIndex)
                index = Random.Range(0, totalNumOfTiles);

            DeleteTile();
            SpawnTile(index);
        }

        // Check if the enemy is instantiated and disable obstacles only once
        if (instantiatedEnemy != null && !obstaclesProcessed)
        {
            DisableObstacles();
            obstaclesProcessed = true; // Set the flag to true to avoid reprocessing
        }
    }

    public void SpawnTile(int index = 0)
    {
        GameObject tile = Instantiate(tilePrefabs[index], Vector3.forward * zSpawn, Quaternion.identity);
        activeTiles.Add(tile);
        zSpawn += tileLength;
        previousIndex = index;
    }

    private void DeleteTile()
    {
        Destroy(activeTiles[0]);
        activeTiles.RemoveAt(0);
    }

    private void DisableObstacles()
    {
        // Only target the nearest tiles to the player
        foreach (GameObject tile in activeTiles)
        {
            if (tile.transform.position.z >= playerTransform.position.z - tileLength && tile.transform.position.z <= playerTransform.position.z + tileLength)
            {
                foreach (Transform child in tile.transform)
                {
                    if (child.CompareTag("Obstacle"))
                    {
                        // Instantiate the particle effect at the obstacle's position
                        if (particleEffectPrefab != null)
                        {
                            GameObject particleEffect = Instantiate(particleEffectPrefab, child.position, Quaternion.identity);
                            // Destroy the particle effect after 2 seconds
                            StartCoroutine(DestroyParticleEffect(particleEffect, 2f));
                        }

                        // Disable the obstacle
                        child.gameObject.SetActive(false);
                    }
                }
            }
            else
            {
                foreach (Transform child in tile.transform)
                {
                    if (child.CompareTag("Obstacle"))
                    {
                        // Just disable the obstacle without particle effect
                        child.gameObject.SetActive(false);
                    }
                }
            }
        }
    }

    private IEnumerator DestroyParticleEffect(GameObject particleEffect, float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(particleEffect);
    }

    // Method to set the instantiated enemy reference
    public void SetInstantiatedEnemy(AIEnemy enemy)
    {
        instantiatedEnemy = enemy;
        obstaclesProcessed = false; // Reset the flag when a new enemy is instantiated
    }
}
