using UnityEngine;

public class WalkBoundZone : MonoBehaviour
{
    [Header("Zone Settings")]
    [SerializeField] private Collider2D zoneCollider;
    [SerializeField] private float moveSpeedInZone = 10f;
    [SerializeField] private ParticleSystem enterEffect; // Optional visual feedback

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerMovement player = other.GetComponent<PlayerMovement>();
            if (player != null)
            {
                player.forcedMoveSpeed = moveSpeedInZone;
                player.EnterWalkBound();
                
                if (enterEffect != null) 
                {
                    enterEffect.Play(); // Play effect if assigned
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerMovement player = other.GetComponent<PlayerMovement>();
            if (player != null)
            {
                player.ExitWalkBound();
            }
        }
    }

    private void OnValidate()
    {
        if (zoneCollider == null)
        {
            zoneCollider = GetComponent<Collider2D>();
        }
    }
}