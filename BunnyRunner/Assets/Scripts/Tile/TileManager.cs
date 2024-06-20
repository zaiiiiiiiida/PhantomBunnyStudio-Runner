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

            // Check and disable obstacles if an enemy has been instantiated
            if (instantiatedEnemy != null)
            {
                DisableObstacles();
            }
        }
    }

    public void SpawnTile(int index = 0)
    {
        GameObject tile = Instantiate(tilePrefabs[index], Vector3.forward * zSpawn, Quaternion.identity);
        activeTiles.Add(tile);
        zSpawn += tileLength;
        previousIndex = index;

        // If an enemy has been instantiated, disable obstacles on newly spawned tile
        if (instantiatedEnemy != null)
        {
            DisableObstacles(tile);
        }
    }

    private void DeleteTile()
    {
        Destroy(activeTiles[0]);
        activeTiles.RemoveAt(0);
    }

    private void DisableObstacles(GameObject tile = null)
    {
        if (tile == null)
        {
            // Disable obstacles on all active tiles
            foreach (GameObject activeTile in activeTiles)
            {
                DisableObstaclesOnTile(activeTile);
            }
        }
        else
        {
            // Disable obstacles on a specific tile
            DisableObstaclesOnTile(tile);
        }
    }

    private void DisableObstaclesOnTile(GameObject tile)
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

    // Method to set the instantiated enemy reference
    public void SetInstantiatedEnemy(AIEnemy enemy)
    {
        instantiatedEnemy = enemy;
        obstaclesProcessed = false; // Reset the flag when a new enemy is instantiated
    }
}
