using UnityEngine;

public class PlayerTriggerMover : MonoBehaviour
{
    [Header("Target Movement Settings")]
    [SerializeField] private Transform objectToMove;
    [SerializeField] private Transform movePointA;
    [SerializeField] private Transform movePointB;
    [SerializeField] private float moveSpeed = 2f;

    [Header("Sprite Settings")]
    [SerializeField] private Sprite inactiveSprite;
    [SerializeField] private Sprite activatedSprite;
    private SpriteRenderer spriteRenderer;

    [Header("Trigger Settings")]
    [SerializeField] private bool deactivateSelfAfter = true;

    private bool hasActivated = false;
    private bool isMoving = false;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null && inactiveSprite != null)
        {
            spriteRenderer.sprite = inactiveSprite;
        }
    }

    private void Update()
    {
        if (isMoving && objectToMove != null)
        {
            objectToMove.position = Vector3.MoveTowards(
                objectToMove.position,
                movePointB.position,
                moveSpeed * Time.deltaTime
            );

            if (Vector3.Distance(objectToMove.position, movePointB.position) < 0.01f)
            {
                isMoving = false;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!hasActivated && collision.CompareTag("Player") && objectToMove != null)
        {
            // Instantly place the object at pointA if not already there
            objectToMove.position = movePointA.position;

            isMoving = true;
            hasActivated = true;

            if (spriteRenderer != null && activatedSprite != null)
            {
                spriteRenderer.sprite = activatedSprite;
            }

            if (deactivateSelfAfter)
                gameObject.SetActive(false);
        }
    }
}
