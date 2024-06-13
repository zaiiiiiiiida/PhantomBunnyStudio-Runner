using System.Collections;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Properties")]
    [SerializeField] private int maxHealth = 3;
    private int currentHealth;

    [Header("Damage Effect")]
    [SerializeField] private GameObject damageEffectPrefab; // Prefab for damage particle effect

    private PlayerController playerController;
    private Animator anim;
    private CameraShake cameraShake;

    void Start()
    {
        currentHealth = maxHealth;
        playerController = GetComponent<PlayerController>();
        anim = GetComponent<Animator>();
        cameraShake = FindObjectOfType<CameraShake>();
        if (cameraShake == null)
        {
            Debug.Log("Camera shake script not found!");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Obstacle"))
        {
            
            StartCoroutine(HandleObstacleCollision(other.gameObject));
        }
    }

    private IEnumerator HandleObstacleCollision(GameObject obstacle)
    {
        TakeDamage();

        
        if (damageEffectPrefab != null)
        {
            GameObject damageEffect = Instantiate(damageEffectPrefab, obstacle.transform.position, Quaternion.identity);
            Destroy(damageEffect, 2f); // Destroy the particle effect after 2 seconds
        }

        // Wait for a short duration to allow the particle effect to play
        yield return new WaitForSeconds(0.5f);

        // Destroy the obstacle
        Destroy(obstacle);
    }

    public void TakeDamage()
    {
        currentHealth--;

        // Shake the camera
        if (cameraShake != null)
        {
            cameraShake.ShakeCamera();
        }
        else
        {
            Debug.Log("Camera shake script not found!");
        }

        // Check if health is depleted
        if (currentHealth <= 0)
        {
            // Stop player movement and perform game over logic
            playerController.enabled = false;
            anim.SetTrigger("Die");
            // You can add further game over logic here
        }
        else
        {
            // Play damage animation without stopping the player
            anim.SetTrigger("TakeDamage");
        }
    }

    public int GetCurrentHealth()
    {
        return currentHealth;
    }
}

