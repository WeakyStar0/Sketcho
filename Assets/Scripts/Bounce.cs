using UnityEngine;

public class Bounce : MonoBehaviour
{
    public float bounceForce = 10f; // The force applied to the player when they hit the trap
    private Animator animator;
    private static readonly int ActiveBounce = Animator.StringToHash("activeBounce");

    private void Start()
    {
        // Get the Animator component attached to this GameObject
        animator = GetComponent<Animator>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            HandlePlayerBounce(collision.gameObject);
            // Trigger the bounce animation
            if (animator != null)
            {
                animator.SetTrigger(ActiveBounce);
            }
        }
    }

    private void HandlePlayerBounce(GameObject player)
    {
        // Apply bounce force to the player
        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();

        if (rb)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
            rb.AddForce(Vector2.up * bounceForce, ForceMode2D.Impulse);
        }
    }
}