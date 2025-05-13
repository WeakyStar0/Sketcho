using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    public Transform pointA; // First point of the platform
    public Transform pointB; // Second point of the platform
    public float moveSpeed = 2f; // Speed of the platform

    private Vector3 nextPosition; // Current target position for the platform
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        nextPosition = pointB.position; // Start moving towards point A
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector3.MoveTowards(
            transform.position,
            nextPosition,
            moveSpeed * Time.deltaTime
        );

        // Check if the platform has reached the target position
        if (transform.position == nextPosition)
        {
            // Switch the target position
            nextPosition = (nextPosition == pointA.position) ? pointB.position : pointA.position;
        }
    }

    private void OllisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // Make the player a child of the platform
            collision.gameObject.transform.parent = transform;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // Remove the player from the platform
            collision.gameObject.transform.parent = null;
        }
    }
}
