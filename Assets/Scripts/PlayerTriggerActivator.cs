using UnityEngine;

public class PlayerTriggerActivator : MonoBehaviour
{
    [Header("Target to Activate")]
    [SerializeField] private GameObject objectToActivate;

    [Header("Sprite Settings")]
    [SerializeField] private Sprite inactiveSprite;
    [SerializeField] private Sprite activatedSprite;
    private SpriteRenderer spriteRenderer;

    [Header("Trigger Settings")]
    [SerializeField] private bool deactivateSelfAfter = true;

    private bool hasActivated = false;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null && inactiveSprite != null)
        {
            spriteRenderer.sprite = inactiveSprite;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!hasActivated && collision.CompareTag("Player") && objectToActivate != null)
        {
            objectToActivate.SetActive(true);
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
