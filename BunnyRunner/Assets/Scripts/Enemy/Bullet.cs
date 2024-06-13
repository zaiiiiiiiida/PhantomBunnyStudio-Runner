using UnityEngine;



public class Bullet : MonoBehaviour
{
    public float speed = 10.0f;
    public float lifeTime = 5.0f;
    public int damageAmount = 1; 

    public GameObject impactEffect;
    PlayerHealth playerHealth;

    private void Start()
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        playerHealth = FindAnyObjectByType<PlayerHealth>();
        if(playerHealth == null)
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

            // Instantiate impact effect
            if (impactEffect != null)
            {
                Instantiate(impactEffect, transform.position, transform.rotation);
            }

            // Destroy bullet
            Destroy(gameObject);
        }
    }
}
