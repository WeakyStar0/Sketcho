using UnityEngine;
using UnityEngine.EventSystems;

public class HoverReveal : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Hover Settings")]
    [SerializeField] private GameObject objectToReveal;
    [SerializeField] private bool keepVisibleAfterHover = false;
    [SerializeField] private float revealDelay = 0.1f;

    [Header("Pulse Animation")]
    [SerializeField] private bool enablePulse = true;
    [SerializeField] private float pulseSpeed = 3f;
    [SerializeField] private float pulseScaleAmount = 0.1f;
    [SerializeField] private Vector3 baseScale = Vector3.one;

    private bool isHovering = false;
    private bool wasRevealed = false;
    private float pulseTimer = 0f;
    private Vector3 originalScale;

    private void Start()
    {
        originalScale = transform.localScale;
        baseScale = originalScale;

        if (objectToReveal != null)
        {
            objectToReveal.SetActive(false);
        }
        else
        {
            Debug.LogWarning("No object assigned to reveal on hover!", this);
        }
    }

    private void Update()
    {
        if (isHovering && enablePulse)
        {
            // Continuous pulsing animation
            pulseTimer += Time.deltaTime * pulseSpeed;
            float scaleFactor = 1f + Mathf.Sin(pulseTimer) * pulseScaleAmount;
            transform.localScale = baseScale * scaleFactor;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovering = true;
        pulseTimer = 0f;
        CancelInvoke(nameof(HideObject));
        Invoke(nameof(ShowObject), revealDelay);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovering = false;
        transform.localScale = baseScale; // Reset scale

        if (!keepVisibleAfterHover || !wasRevealed)
        {
            CancelInvoke(nameof(ShowObject));
            Invoke(nameof(HideObject), revealDelay);
        }
    }

    private void ShowObject()
    {
        if (isHovering && objectToReveal != null)
        {
            objectToReveal.SetActive(true);
            wasRevealed = true;
        }
    }

    private void HideObject()
    {
        if (!isHovering && objectToReveal != null && !keepVisibleAfterHover)
        {
            objectToReveal.SetActive(false);
            wasRevealed = false;
        }
    }

    // Manual control methods
    public void ForceShow()
    {
        ShowObject();
        isHovering = true;
    }

    public void ForceHide()
    {
        HideObject();
        isHovering = false;
        transform.localScale = baseScale;
    }

    private void OnDisable()
    {
        // Reset when disabled
        transform.localScale = baseScale;
        isHovering = false;
    }
}