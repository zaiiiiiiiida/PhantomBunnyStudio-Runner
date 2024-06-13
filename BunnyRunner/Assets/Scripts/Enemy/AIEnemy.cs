using System.Collections;
using UnityEngine;

public class AIEnemy : MonoBehaviour
{
    [Header("Enemy Properties")]
    public string playerTag = "Player"; // Tag to find the player
    public string obstacleTag = "Obstacle"; // Tag to identify obstacles
    private Transform player;
    public float minChaseSpeed = 5.0f; // Minimum chase speed
    public float maxChaseSpeed = 7.5f; // Maximum chase speed
    public float minFollowDistance = 1f; // Minimum follow distance
    public float maxFollowDistance = 3f; // Maximum follow distance
    public float laneSwitchSpeed = 10.0f;
    public float laneDistance = 2.5f;

    [Header("Obstacle Avoidance")]
    public float detectionDistance = 2.0f; // Distance to detect obstacles

    [Header("AI Throwing")]
    public float needleSpawnIntervalMin = 1.0f; // Minimum interval between shooting needles
    public float needleSpawnIntervalMax = 2.5f; // Maximum interval between shooting needles
    public GameObject needlePrefab; // Prefab for the needle
    public Transform spawnPoint; // Spawn point for the needle
    public float needleSpawnDistance = 1.0f; // Distance from enemy to spawn needle

    [Header("Timers")]
    public float timerMaxChaseStart = 5f; // Time before the enemy starts increasing speed
    public float speedChangeDuration = 2.0f; // Duration for changing speed

    [Header("Stamina")]
    public float maxStamina = 100f; // Maximum stamina
    public float staminaIncreaseRate = 5f; // Stamina increase rate per second
    public float staminaDecreaseRate = 10f; // Stamina decrease rate per second
    private float currentStamina;

    private Animator anim;
    private PlayerHealth playerHealth;
    private PlayerController playerController;
    private float currentChaseSpeed;
    private float currentFollowDistance;

    // Player attack flag
    public bool playerCanAttack = false;

    // Enemy health
    public int health = 100;

    [Header("Effects")]
    public GameObject deathParticleEffect; // Particle effect to play on death

    void Start()
    {
        playerHealth = FindAnyObjectByType<PlayerHealth>();
        if (playerHealth == null)
        {
            Debug.Log("PlayerHealthScript not found");
        }

        anim = GetComponent<Animator>();
        if (anim == null)
        {
            Debug.LogError("Animator not found! Please add an Animator component to the enemy GameObject.");
        }

        currentChaseSpeed = maxChaseSpeed;
        currentFollowDistance = maxFollowDistance;
        currentStamina = maxStamina;
        StartCoroutine(AdjustSpeedAndFollowDistance());
        StartCoroutine(ThrowNeedles());
    }

    public void Initialize(Transform playerTransform)
    {
        player = playerTransform;
        playerController = FindAnyObjectByType<PlayerController>();
    }

    void Update()
    {
        if (player != null && playerHealth != null && playerHealth.GetCurrentHealth() > 0)
        {
            FollowPlayer();
            UpdateStamina();
            CheckDistanceToPlayer();
        }
        else
        {
            if (anim != null)
            {
                anim.SetTrigger("Dance");
            }
        }
    }

    private void FollowPlayer()
    {
        int playerLane = 1; // Default to middle lane
        if (player.position.x < -laneDistance / 2)
        {
            playerLane = 0; // Left lane
        }
        else if (player.position.x > laneDistance / 2)
        {
            playerLane = 2; // Right lane
        }

        Vector3 targetPosition = new Vector3((playerLane - 1) * laneDistance, transform.position.y, player.position.z - currentFollowDistance);
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, currentChaseSpeed * Time.deltaTime);

        if (anim != null)
        {
            anim.SetTrigger("RunForward");
        }
    }

    private IEnumerator AdjustSpeedAndFollowDistance()
    {
        yield return new WaitForSeconds(timerMaxChaseStart);

        while (true)
        {
            float elapsedTime = 0f;
            float startDistance = maxFollowDistance;
            float endDistance = minFollowDistance;
            float startSpeed = minChaseSpeed;
            float endSpeed = maxChaseSpeed;

            while (elapsedTime < speedChangeDuration)
            {
                currentFollowDistance = Mathf.Lerp(startDistance, endDistance, elapsedTime / speedChangeDuration);
                currentChaseSpeed = Mathf.Lerp(startSpeed, endSpeed, elapsedTime / speedChangeDuration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            currentFollowDistance = endDistance;
            currentChaseSpeed = endSpeed;

            yield return new WaitForSeconds(2f);

            elapsedTime = 0f;
            startDistance = minFollowDistance;
            endDistance = maxFollowDistance;
            startSpeed = maxChaseSpeed;
            endSpeed = minChaseSpeed;

            while (elapsedTime < speedChangeDuration)
            {
                currentFollowDistance = Mathf.Lerp(startDistance, endDistance, elapsedTime / speedChangeDuration);
                currentChaseSpeed = Mathf.Lerp(startSpeed, endSpeed, elapsedTime / speedChangeDuration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            currentFollowDistance = endDistance;
            currentChaseSpeed = endSpeed;

            yield return new WaitForSeconds(2f);
        }
    }

    private IEnumerator ThrowNeedles()
    {
        while (true)
        {
            if (playerHealth == null || playerHealth.GetCurrentHealth() <= 0)
            {
                yield break; // Exit the coroutine if player's health is 0
            }

            float needleSpawnInterval = Mathf.Lerp(needleSpawnIntervalMax, needleSpawnIntervalMin, (currentFollowDistance - minFollowDistance) / (maxFollowDistance - minFollowDistance));
            yield return new WaitForSeconds(needleSpawnInterval);
            AIThrowObject();
        }
    }

    private void AIThrowObject()
    {
        if (needlePrefab != null && spawnPoint != null)
        {
            Instantiate(needlePrefab, spawnPoint.position, spawnPoint.rotation);

            if (anim != null)
            {
                anim.SetTrigger("RunAttack");
            }
        }
    }

    private void UpdateStamina()
    {
        if (currentStamina < maxStamina)
        {
            currentStamina += staminaIncreaseRate * Time.deltaTime;
            currentStamina = Mathf.Clamp(currentStamina, 0f, maxStamina);
        }
    }

    private void CheckDistanceToPlayer()
    {
        if (player != null)
        {
            float distanceToPlayerZ = Mathf.Abs(transform.position.z - player.position.z);

            if (distanceToPlayerZ <= maxFollowDistance)
            {
                playerCanAttack = true;
                Debug.Log($"Enemy is within max follow distance. Z Distance: {distanceToPlayerZ}. Player can attack: {playerCanAttack}");
            }
            else
            {
                playerCanAttack = false;
                Debug.Log($"Enemy is outside max follow distance. Z Distance: {distanceToPlayerZ}. Player can attack: {playerCanAttack}");
            }
        }
    }

    public void EnemyTakesDamage(int damage)
    {
        health -= damage;
        anim.SetTrigger("TakeDamage");
        health = Mathf.Clamp(health, 0, int.MaxValue);
        if (health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        if (playerController != null)
        {
            playerController.OnEnemyDeath();
        }

        playerCanAttack = false;

        if (anim != null)
        {
            anim.SetTrigger("Die");
        }

        // Play death particle effect if available
        if (deathParticleEffect != null)
        {
            Instantiate(deathParticleEffect, transform.position, Quaternion.identity);
        }

        // Optionally, disable the enemy game object after a delay to allow the death animation to play
        StartCoroutine(DisableAfterDelay(1.0f)); // 1 second delay as an example
    }

    private IEnumerator DisableAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        gameObject.SetActive(false);
    }
}
