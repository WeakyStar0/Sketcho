using Unity.VisualScripting;
using UnityEngine;

public class Spikes : MonoBehaviour
{
    public float bounceForce = 10f; // The force applied to the player when they hit the trap
    public float damage = 1f; // The damage dealt to the player when they hit the trap

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            HandlePlayerBounce(collision.gameObject);
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
