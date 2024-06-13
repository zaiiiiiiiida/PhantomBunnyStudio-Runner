using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CameraController : MonoBehaviour
{
    private Transform target;
    private Vector3 initialOffset;
    private Vector3 targetOffset;
    private Vector3 currentOffset;
    public GameObject enemyPrefab;
    public GameObject particleEffectPrefab;
    private Transform playerTransform;
    [SerializeField] private float waitingTime = 120.0f;
    public float transitionDuration = 2.0f;
    private bool cameraReversed = false;
    private PlayerController playerController;
    private TileManager tileManager; // Reference to TileManager

    public float survivalTime = 30.0f;

    public Slider waitingTimeSlider;

    void Start()
    {
        target = GameObject.FindGameObjectWithTag("Player").transform;
        initialOffset = transform.position - target.position;
        targetOffset = new Vector3(0, initialOffset.y, -initialOffset.z);
        currentOffset = initialOffset;
        playerTransform = target;
        playerController = target.GetComponent<PlayerController>();
        tileManager = FindObjectOfType<TileManager>(); // Find TileManager instance

        if (waitingTimeSlider != null)
        {
            waitingTimeSlider.maxValue = waitingTime;
            waitingTimeSlider.value = 0;
        }

        StartCoroutine(TransitionToFrontSide());
    }

    void LateUpdate()
    {
        Vector3 newPosition = target.position + currentOffset;
        transform.position = Vector3.Lerp(transform.position, newPosition, 0.6f);
        transform.LookAt(target);
    }

    private IEnumerator TransitionToFrontSide()
    {
        float elapsedTime = 0f;

        while (elapsedTime < waitingTime)
        {
            if (waitingTimeSlider != null)
            {
                waitingTimeSlider.value = elapsedTime;
            }
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        if (waitingTimeSlider != null)
        {
            waitingTimeSlider.value = waitingTime;
        }

        if (!cameraReversed)
        {
            cameraReversed = true;
            elapsedTime = 0f;

            while (elapsedTime < transitionDuration)
            {
                currentOffset = Vector3.Lerp(initialOffset, targetOffset, elapsedTime / transitionDuration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            currentOffset = targetOffset;

            if (playerController != null)
            {
                playerController.ReverseControls();
            }

            InstantiateEnemy();

            if (particleEffectPrefab != null)
            {
                Instantiate(particleEffectPrefab, target.position, Quaternion.identity);
            }
        }
    }

    private void InstantiateEnemy()
    {
        if (enemyPrefab != null && playerTransform != null)
        {
            Vector3 spawnOffset = -playerTransform.forward * 5f;
            Vector3 spawnPosition = playerTransform.position + spawnOffset;

            GameObject enemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
            AIEnemy aiEnemy = enemy.GetComponent<AIEnemy>();
            if (aiEnemy != null)
            {
                aiEnemy.Initialize(playerTransform);
                // Notify PlayerController of the new enemy
                playerController.SetEnemyScript(aiEnemy);
                // Notify TileManager of the new enemy
                if (tileManager != null)
                {
                    tileManager.SetInstantiatedEnemy(aiEnemy);
                }
            }
            Debug.Log("Enemy instantiated at: " + spawnPosition);
        }
        else
        {
            Debug.LogWarning("Enemy Prefab or Player Transform is not assigned!");
        }
    }
}
