using UnityEngine;

public class EraserEnemyPatrol : MonoBehaviour
{
    [Header("Patrol Settings")]
    [SerializeField] private Transform pointA;
    [SerializeField] private Transform pointB;
    [SerializeField] private float speed = 2f;
    [SerializeField] private float waitTime = 1f; // Time to wait at each point
    
    [Header("Visuals")]
    [SerializeField] private bool flipSprite = true; // Enable/disable sprite flipping
    
    private Transform currentTarget;
    private SpriteRenderer spriteRenderer;
    private bool isWaiting = false;
    private float waitTimer = 0f;

    public int damage = 1; // Damage dealt to the player

    private void Start()
    {
        // Initialize by moving toward point A
        currentTarget = pointA;
        spriteRenderer = GetComponent<SpriteRenderer>();
        
        // Ensure the sprite faces the initial direction
        UpdateSpriteDirection();
    }

    private void Update()
    {
        if (isWaiting)
        {
            // Handle waiting at a point
            waitTimer += Time.deltaTime;
            if (waitTimer >= waitTime)
            {
                isWaiting = false;
                waitTimer = 0f;
                SwitchTarget();
            }
        }
        else
        {
            // Move toward the current target
            transform.position = Vector2.MoveTowards(
                transform.position, 
                currentTarget.position, 
                speed * Time.deltaTime
            );

            // Check if reached the current target
            if (Vector2.Distance(transform.position, currentTarget.position) < 0.1f)
            {
                isWaiting = true;
            }
            
            // Update sprite direction while moving
            UpdateSpriteDirection();
        }
    }

    private void SwitchTarget()
    {
        // Switch to the other target point
        currentTarget = currentTarget == pointA ? pointB : pointA;
    }

    private void UpdateSpriteDirection()
    {
        if (!flipSprite || spriteRenderer == null) return;
        
        // Flip sprite based on movement direction
        if (currentTarget.position.x > transform.position.x)
        {
            spriteRenderer.flipX = true;
        }
        else if (currentTarget.position.x < transform.position.x)
        {
            spriteRenderer.flipX = false;
        }
    }

    // Visualize the patrol path in the editor
    private void OnDrawGizmos()
    {
        if (pointA != null && pointB != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(pointA.position, pointB.position);
            Gizmos.DrawSphere(pointA.position, 0.2f);
            Gizmos.DrawSphere(pointB.position, 0.2f);
            
            // Draw a line to current target in Scene view
            if (Application.isPlaying && currentTarget != null)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawLine(transform.position, currentTarget.position);
            }
        }
    }
}