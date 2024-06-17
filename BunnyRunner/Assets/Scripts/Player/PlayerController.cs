using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    private CharacterController controller;
    private Vector3 move;
    public float forwardSpeed;
    public float maxSpeed;
    public float lateralMoveSpeed = 10.0f;

    public int desiredLane = 1; // 0:left, 1:middle, 2:right
    public float laneDistance = 2.5f;

    public float gravity = -20f;
    public float jumpHeight = 2f;
    private Vector3 velocity;

    private bool isGrounded = true;
    private bool isSliding = false;

    public float slideDuration = 1.5f;

    bool toggle = false;

    [Header("AnimationSettings")]
    public Animator anim;
    private bool isCameraReversed = false;
    public int health = 100;
    public Transform enemyTransform;
    private AIEnemy enemyScript;
    public Button attackButton;
    private PlayerManager playerManager;
    void Start()
    {
        controller = GetComponent<CharacterController>();
        anim = GetComponent<Animator>();
        Time.timeScale = 1.2f;
        playerManager = FindObjectOfType<PlayerManager>();
        if (playerManager != null)
        {
            Debug.Log("PlayerManager is found");
        }

        // Add listener to the attack button
        if (attackButton != null)
        {
            attackButton.onClick.AddListener(Attack);
            attackButton.gameObject.SetActive(false); // Initially set the button to be not visible
        }
    }

    private void FixedUpdate()
    {
        if (toggle)
        {
            toggle = false;
            if (forwardSpeed < maxSpeed)
                forwardSpeed += 0.1f * Time.fixedDeltaTime;
        }
        else
        {
            toggle = true;
            if (Time.timeScale < 2f)
                Time.timeScale += 0.005f * Time.fixedDeltaTime;
        }
    }

    void Update()
    {
        isGrounded = controller.isGrounded;
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        if (SwipeManager.swipeUp && isGrounded)
        {
            Jump();
        }

        if (SwipeManager.swipeDown && !isSliding)
        {
            StartCoroutine(Slide());
        }

        if (isCameraReversed)
        {
            if (SwipeManager.swipeRight)
            {
                desiredLane--;
                anim.SetTrigger("TurnLeft");
                if (desiredLane == -1)
                {
                    desiredLane = 0;
                }
            }

            if (SwipeManager.swipeLeft)
            {
                anim.SetTrigger("TurnRight");
                desiredLane++;
                if (desiredLane == 3)
                {
                    desiredLane = 2;
                }
            }
        }
        else
        {
            if (SwipeManager.swipeRight)
            {
                desiredLane++;
                anim.SetTrigger("TurnRight");
                if (desiredLane == 3)
                {
                    desiredLane = 2;
                }
            }

            if (SwipeManager.swipeLeft)
            {
                anim.SetTrigger("TurnLeft");
                desiredLane--;
                if (desiredLane == -1)
                {
                    desiredLane = 0;
                }
            }
        }

        // Calculate where we should be in the future
        Vector3 targetPosition = new Vector3((desiredLane - 1) * laneDistance, transform.position.y, transform.position.z);

        // Only update the X position, keep Y and Z as is
        Vector3 moveDir = (targetPosition - transform.position);
        moveDir.y = 0; // Ensure no change in the Y axis

        if (moveDir.magnitude > 0.1f)
        {
            controller.Move(moveDir.normalized * lateralMoveSpeed * Time.deltaTime);
        }

        // Update the forward movement
        move = transform.forward * forwardSpeed;
        controller.Move(move * Time.deltaTime);

        if (isGrounded)
        {
            anim.SetTrigger("RunForward");
        }

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        // Check the distance to the enemy and update attack button visibility
        if (enemyScript != null)
        {
            UpdateAttackButtonVisibility(enemyScript.playerCanAttack);
        }
    }

    private void Jump()
    {
        StopCoroutine(Slide());
        anim.SetTrigger("Jump");
        velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        isSliding = false;
    }

    private IEnumerator Slide()
    {
        isSliding = true;
        anim.SetTrigger("Sliding");
        controller.height = 1;
        controller.center = new Vector3(0, -0.5f, 0);

        yield return new WaitForSeconds(slideDuration / Time.timeScale);

        controller.height = 2;
        controller.center = Vector3.zero;
        isSliding = false;
    }

    // Method to reverse the controls
    public void ReverseControls()
    {
        isCameraReversed = !isCameraReversed;
    }


    private void UpdateAttackButtonVisibility(bool canAttack)
    {
        if (attackButton != null)
        {
            attackButton.gameObject.SetActive(canAttack);
        }
    }

    // Method to handle player attack
    public void Attack()
    {
        if (enemyScript != null && enemyScript.playerCanAttack)
        {
            anim.SetTrigger("Attacking");
            enemyScript.EnemyTakesDamage(25);
        }
    }

    // Method to update the enemy script reference
    public void SetEnemyScript(AIEnemy newEnemyScript)
    {
        enemyScript = newEnemyScript;
        if (enemyScript != null)
        {
            enemyTransform = enemyScript.transform;
        }
    }

    // Method to handle enemy death
    public void OnEnemyDeath()
    {
        if (attackButton != null)
        {
            attackButton.gameObject.SetActive(false);
        }
    }

    public void OnEnemyDefeated()
    {
        // Stop player movement
        forwardSpeed = 0f;

        // Trigger the "Dance" animation
        anim.SetTrigger("Dance");

        // Show the winner pop-up
        if (playerManager != null)
        {
            playerManager.ShowWinnerPopup();
        }
    }
}

