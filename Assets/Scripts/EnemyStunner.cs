using UnityEngine;

public class EnemyStunner : MonoBehaviour
{
    [SerializeField] private Color flashColor = Color.red;
    [SerializeField] private float stunDuration = 1f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        FloatingEnemyPatrol enemy = other.GetComponent<FloatingEnemyPatrol>();
        if (enemy != null)
        {
            enemy.Stun(stunDuration, flashColor);
        }
    }
}
