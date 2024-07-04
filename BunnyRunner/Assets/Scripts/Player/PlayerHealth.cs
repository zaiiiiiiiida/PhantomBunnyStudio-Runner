using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Properties")]
    [SerializeField] private int maxHealth = 3;
    private int currentHealth;

    [Header("Damage Effect")]
    [SerializeField] private GameObject damageEffectPrefab;

    [Header("UI References")]
    public Image healthBar;

    private PlayerController playerController;
    private Animator anim;
    private CameraShake cameraShake;
    public GameObject gameOverPanel;
    public Button reviveButton;
    private AdsManager adManager;

    void Start()
    {
        currentHealth = maxHealth;
        playerController = GetComponent<PlayerController>();
        UpdateAnimatorReference();
        cameraShake = FindObjectOfType<CameraShake>();
        if (cameraShake == null)
        {
            Debug.Log("Camera shake script not found!");
        }

        adManager = FindObjectOfType<AdsManager>();
        if (adManager != null && reviveButton != null)
        {
            reviveButton.onClick.AddListener(adManager.ShowRewardedAdRevive);
        }

        UpdateHealthBar();
    }

    public void UpdateAnimatorReference()
    {
        // Find the currently active character and get its Animator
        foreach (Transform child in transform)
        {
            if (child.gameObject.activeSelf)
            {
                anim = child.GetComponent<Animator>();
                break;
            }
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
            Destroy(damageEffect, 2f);
        }

        yield return new WaitForSeconds(0.5f);

        Destroy(obstacle);
    }

    public void TakeDamage()
    {
        currentHealth--;

        UpdateHealthBar();

        if (cameraShake != null)
        {
            cameraShake.ShakeCamera();
        }
        else
        {
            Debug.Log("Camera shake script not found!");
        }

        if (currentHealth <= 0)
        {
            playerController.enabled = false;
            anim.SetTrigger("Die");
            gameOverPanel.SetActive(true);
            if (reviveButton != null)
            {
                reviveButton.gameObject.SetActive(true);
            }
        }
        else
        {
            anim.SetTrigger("TakeDamage");
        }
    }

    public void Revive()
    {
        currentHealth = maxHealth;

        UpdateHealthBar();

        playerController.enabled = true;
        anim.ResetTrigger("Die");
        anim.SetTrigger("Revive");

        gameOverPanel.SetActive(false);
        if (reviveButton != null)
        {
            reviveButton.gameObject.SetActive(false);
        }

        AIEnemy enemy = FindObjectOfType<AIEnemy>();
        if (enemy == null)
        {
            Debug.Log("Can't access the enemy");
        }
        if (enemy != null)
        {
            enemy.OnPlayerRevived();
        }
        else
        {
            enemy = FindAnyObjectByType<AIEnemy>();
        }
    }

    private void UpdateHealthBar()
    {
        if (healthBar != null)
        {
            healthBar.fillAmount = (float)currentHealth / maxHealth;
        }
    }

    public int GetCurrentHealth()
    {
        return currentHealth;
    }
}
