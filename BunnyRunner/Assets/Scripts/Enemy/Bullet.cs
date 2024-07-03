using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 10.0f;
    public float lifeTime = 5.0f;
    public int damageAmount = 1;

    public GameObject impactEffect;
    public Vector3 impactEffectOffset = Vector3.zero; // Offset for impact effect
    public Vector3 impactEffectRotation = Vector3.zero; // Rotation for impact effect

    public AudioSource audioSource; // Reference to AudioSource component
    public AudioClip hitSound; // Sound clip for hit effect

    private PlayerHealth playerHealth;

    private void Start()
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        playerHealth = FindAnyObjectByType<PlayerHealth>();

        if (playerHealth == null)
        {
            Debug.Log("Player health == null");
        }

        if (rb != null)
        {
            rb.useGravity = false;
            rb.velocity = transform.forward * speed;
        }

        Destroy(gameObject, lifeTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (playerHealth != null)
            {
                playerHealth.TakeDamage();
            }

            PlayHitSound(); // Play hit sound

            InstantiateImpactEffect(); // Instantiate impact effect

            Destroy(gameObject); // Destroy bullet
        }
    }

    private void PlayHitSound()
    {
        if (audioSource != null && hitSound != null)
        {
            audioSource.PlayOneShot(hitSound);
        }
    }

    private void InstantiateImpactEffect()
    {
        // Instantiate impact effect
        if (impactEffect != null)
        {
            // Calculate position and rotation for the impact effect
            Vector3 effectPosition = transform.position + impactEffectOffset;
            Quaternion effectRotation = Quaternion.Euler(impactEffectRotation);

            Instantiate(impactEffect, effectPosition, effectRotation);
        }
    }

    private T FindAnyObjectByType<T>() where T : MonoBehaviour
    {
        T[] objects = FindObjectsOfType<T>();
        if (objects.Length > 0)
        {
            return objects[0];
        }
        return null;
    }
}
