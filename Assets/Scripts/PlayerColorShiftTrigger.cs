using System.Collections;
using UnityEngine;

public class PlayerColorShiftTrigger : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private SpriteRenderer targetRenderer;

    [Header("Color Settings")]
    [SerializeField] private Color color1 = Color.white;
    [SerializeField] private Color color2 = Color.gray;
    [SerializeField] private float interval = 0.5f;

    private bool isShifting = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isShifting && collision.CompareTag("Player") && targetRenderer != null)
        {
            StartCoroutine(ColorShiftLoop());
        }
    }

    private IEnumerator ColorShiftLoop()
    {
        isShifting = true;
        bool toggle = false;

        while (true)
        {
            targetRenderer.color = toggle ? color1 : color2;
            toggle = !toggle;
            yield return new WaitForSeconds(interval);
        }
    }
}
