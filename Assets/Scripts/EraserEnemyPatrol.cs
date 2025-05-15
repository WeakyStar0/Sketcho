using UnityEngine;

public class EraserEnemyPatrol : MonoBehaviour
{
    [Header("Patrol Settings")]
    [SerializeField] private Transform pointA;
    [SerializeField] private Transform pointB;
    [SerializeField] private float speed = 2f;
    [SerializeField] private float waitTime = 1f;
    
    [Header("Collision Settings")]
    [SerializeField] private bool canBePushed = false;
    [SerializeField] private float pushResistance = 5f;
    [SerializeField] private Collider2D damageTrigger; // Assign in Inspector

    [Header("Visuals")]
    [SerializeField] private bool flipSprite = true;

    private Transform currentTarget;
    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rb;
    private bool isWaiting = false;
    private float waitTimer = 0f;

    public int damage = 1;

    private void Start()
    {
        currentTarget = pointA;
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();

        if (rb == null) rb = gameObject.AddComponent<Rigidbody2D>();
        rb.gravityScale = 0;
        rb.freezeRotation = true;

        // Auto-setup if missing
        if (damageTrigger == null) damageTrigger = GetComponent<Collider2D>();
        if (damageTrigger != null) damageTrigger.isTrigger = true;
    }

    private void Update()
    {
        if (isWaiting)
        {
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
            // Only move if not being pushed
            if (rb.linearVelocity.magnitude < 0.1f)
            {
                transform.position = Vector2.MoveTowards(
                    transform.position, 
                    currentTarget.position, 
                    speed * Time.deltaTime
                );
            }

            if (Vector2.Distance(transform.position, currentTarget.position) < 0.1f)
            {
                isWaiting = true;
            }
            
            UpdateSpriteDirection();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (!canBePushed)
            {
                rb.linearVelocity = Vector2.zero;
                rb.isKinematic = true;
            }
            else
            {
                Vector2 pushDirection = (transform.position - collision.transform.position).normalized;
                rb.AddForce(pushDirection * pushResistance, ForceMode2D.Impulse);
            }
        }
    }

    private void SwitchTarget() => currentTarget = currentTarget == pointA ? pointB : pointA;

    private void UpdateSpriteDirection()
    {
        if (!flipSprite || spriteRenderer == null) return;
        spriteRenderer.flipX = currentTarget.position.x > transform.position.x;
    }

    private void OnDrawGizmos()
    {
        if (pointA != null && pointB != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(pointA.position, pointB.position);
            Gizmos.DrawSphere(pointA.position, 0.2f);
            Gizmos.DrawSphere(pointB.position, 0.2f);
        }
    }
}