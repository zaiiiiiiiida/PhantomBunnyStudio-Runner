using System.Collections;
using UnityEngine;

public class Loot : MonoBehaviour
{
    [Header("RotationProperties")]
    [SerializeField] private float rotationSpeed;

    [Header("Effect")]
    [SerializeField] private GameObject collectEffectPrefab; // The prefab asset for the particle effect

    [Header("Sound")]
    [SerializeField] private AudioClip collectSound; // The sound to play on collection

    private void Update()
    {
        transform.Rotate(rotationSpeed * Time.deltaTime, 0f, 0f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerManager playerManager = FindObjectOfType<PlayerManager>();
            playerManager.AddScore(1);

            // Play the collect sound at the loot's position
            if (collectSound != null)
            {
                AudioSource.PlayClipAtPoint(collectSound, transform.position);
            }
            else
            {
                Debug.LogWarning("Collect sound is not set.");
            }

            // Instantiate the particle effect at the loot's position
            GameObject effectInstance = Instantiate(collectEffectPrefab, transform.position, Quaternion.identity);
            ParticleSystem particleSystem = effectInstance.GetComponent<ParticleSystem>();

            if (particleSystem != null)
            {
                particleSystem.Play();
                // Start coroutine to destroy the effect after playing
                StartCoroutine(DestroyParticleSystem(effectInstance, particleSystem));
            }
            else
            {
                Debug.LogError("The collectEffectPrefab does not contain a ParticleSystem component!");
                Destroy(effectInstance);
            }

            Destroy(gameObject);
        }
    }

    private IEnumerator DestroyParticleSystem(GameObject effectInstance, ParticleSystem particleSystem)
    {
        // Wait for the particle system to finish
        yield return new WaitWhile(() => particleSystem.IsAlive(true));

        // Destroy the particle system's GameObject
        Destroy(effectInstance);
    }
}
