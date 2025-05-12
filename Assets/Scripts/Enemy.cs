using UnityEngine;

public class Enemy : MonoBehaviour
{
    public Transform target; // The target to follow
    public float speed = 2f; // Speed of the enemy
    public float jumpForce = 2f; // Jump force of the enemy
    public LayerMask groundLayer; // Layer mask for the ground

    private Rigidbody2D rb; // Reference to the Rigidbody2D component
    private bool isGrounded; // Check if the enemy is grounded
    private bool shouldJump; // Check if the enemy should jump
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>(); // Get the Rigidbody2D component
    }

    // Update is called once per frame
    void Update()
    {
        isGrounded = Physics2D.Raycast(transform.position, Vector2.down, 1f, groundLayer); // Check if the enemy is grounded

        float direction = Mathf.Sign(target.position.x - transform.position.x); // Get the direction to the target

        bool isPlayerAbove = Physics2D.Raycast(transform.position, Vector2.up, 3f, 1 << target.gameObject.layer); // Check if the player is above

        if (isGrounded)
        {
            rb.linearVelocity = new Vector2(direction * speed, rb.linearVelocity.y); // Move the enemy towards the target

            RaycastHit2D groundInFront = Physics2D.Raycast(transform.position, new Vector2(direction, 0), 2f, groundLayer); // Check if there is ground in front of the enemy

            RaycastHit2D gapAhead = Physics2D.Raycast(transform.position + new Vector3(direction, 0, 0), Vector2.down, 2f, groundLayer); // Check if there is a gap ahead

            RaycastHit2D platformAbove = Physics2D.Raycast(transform.position, Vector2.up, 3f, groundLayer); // Check if there is a platform above

            if (!groundInFront.collider && !gapAhead.collider)
            {
                shouldJump = true; // Set shouldJump to true if there is no ground in front and no gap ahead
            }
            else if (isPlayerAbove && platformAbove.collider)
            {
                shouldJump = true; // Set shouldJump to true if the player is above and there is a platform above
            }
        }
    }

    private void FixedUpdate()
    {
        if (isGrounded && shouldJump)
        {
            shouldJump = false; // Reset shouldJump
            Vector2 jumpVelocity =(target.position - transform.position).normalized; // Calculate the jump velocity

            Vector2 jumpDirection = jumpVelocity * jumpForce; // Calculate the jump direction

            rb.AddForce(new Vector2(jumpDirection.x, jumpForce), ForceMode2D.Impulse); // Apply the jump force
        }
    }
}
