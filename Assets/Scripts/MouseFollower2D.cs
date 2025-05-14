using UnityEngine;

public class MouseFollower2D : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float smoothTime = 0.1f;
    [SerializeField] private float minFollowDistance = 0.5f;

    [Header("Animation")]
    [SerializeField] private Animator animator;
    [SerializeField] private string walkBoolName = "IsWalking";
    [SerializeField] private float walkThreshold = 0.1f;

    [Header("Visuals")]
    [SerializeField] private Transform characterSprite;
    [SerializeField] private float flipThreshold = 0.1f;

    private Vector2 currentVelocity;
    private float fixedYPosition;
    private bool isFacingRight = true;
    private Camera mainCam;
    private bool isWalking;

    private void Start()
    {
        mainCam = Camera.main;
        fixedYPosition = transform.position.y;
        if (characterSprite == null) characterSprite = transform;
    }

    private void Update()
    {
        Vector2 targetPos = mainCam.ScreenToWorldPoint(Input.mousePosition);
        targetPos.y = fixedYPosition;

        if (Vector2.Distance(transform.position, targetPos) > minFollowDistance)
        {
            transform.position = Vector2.SmoothDamp(
                transform.position,
                targetPos,
                ref currentVelocity,
                smoothTime,
                moveSpeed
            );
        }
        else
        {
            currentVelocity = Vector2.Lerp(currentVelocity, Vector2.zero, 10f * Time.deltaTime);
        }

        UpdateAnimation();
        UpdateFacingDirection(targetPos);
    }

    private void UpdateAnimation()
    {
        isWalking = currentVelocity.magnitude > walkThreshold;
        
        if (animator != null)
        {
            animator.SetBool(walkBoolName, isWalking);
        }
    }

    private void UpdateFacingDirection(Vector2 targetPos)
    {
        if (characterSprite == null) return;

        float xDiff = targetPos.x - transform.position.x;
        if (Mathf.Abs(xDiff) > flipThreshold)
        {
            bool shouldFaceRight = xDiff > 0;
            
            if (shouldFaceRight != isFacingRight)
            {
                isFacingRight = shouldFaceRight;
                Vector3 scale = characterSprite.localScale;
                scale.x = Mathf.Abs(scale.x) * (isFacingRight ? 1 : -1);
                characterSprite.localScale = scale;
            }
        }
    }
}