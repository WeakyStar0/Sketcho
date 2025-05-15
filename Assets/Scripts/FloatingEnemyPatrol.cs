using System.Collections;
using UnityEngine;

public class FloatingEnemyPatrol : MonoBehaviour
{
    [Header("Patrol Settings")]
    [SerializeField] private Transform[] patrolPoints;
    [SerializeField] private float speed = 2f;
    [SerializeField] private float waitTime = 1f;

    [Header("Visuals")]
    [SerializeField] private bool flipSprite = true;

    [Header("Combat")]
    public int damage = 1;

    private int currentPointIndex = 0;
    private bool isWaiting = false;
    private float waitTimer = 0f;
    private bool isStunned = false;

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb == null) rb = gameObject.AddComponent<Rigidbody2D>();
        rb.gravityScale = 0;
        rb.freezeRotation = true;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;

        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        if (isStunned || patrolPoints.Length == 0) return;

        if (isWaiting)
        {
            waitTimer += Time.deltaTime;
            if (waitTimer >= waitTime)
            {
                isWaiting = false;
                waitTimer = 0f;
                GoToNextPoint();
            }
            return;
        }

        Transform target = patrolPoints[currentPointIndex];
        transform.position = Vector2.MoveTowards(transform.position, target.position, speed * Time.deltaTime);

        if (Vector2.Distance(transform.position, target.position) < 0.05f)
        {
            isWaiting = true;
        }

        UpdateSpriteDirection(target);
    }

    private void GoToNextPoint()
    {
        currentPointIndex = (currentPointIndex + 1) % patrolPoints.Length;
    }

    private void UpdateSpriteDirection(Transform target)
    {
        if (!flipSprite || spriteRenderer == null) return;
        spriteRenderer.flipX = target.position.x > transform.position.x;
    }

    public void Stun(float duration, Color flashColor)
    {
        if (!gameObject.activeInHierarchy) return;
        StartCoroutine(DoStun(duration, flashColor));
    }

    private IEnumerator DoStun(float duration, Color flashColor)
    {
        isStunned = true;

        if (spriteRenderer != null)
            spriteRenderer.color = flashColor;

        yield return new WaitForSeconds(duration);

        if (spriteRenderer != null)
            spriteRenderer.color = Color.white;

        isStunned = false;
    }

    private void OnDrawGizmosSelected()
    {
        if (patrolPoints == null || patrolPoints.Length == 0) return;

        Gizmos.color = Color.cyan;
        for (int i = 0; i < patrolPoints.Length; i++)
        {
            if (patrolPoints[i] == null) continue;

            Gizmos.DrawSphere(patrolPoints[i].position, 0.15f);

            if (i < patrolPoints.Length - 1 && patrolPoints[i + 1] != null)
                Gizmos.DrawLine(patrolPoints[i].position, patrolPoints[i + 1].position);
        }
    }
}
