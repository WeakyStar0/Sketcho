using UnityEngine;

public class ItemFloat : MonoBehaviour
{
    [Header("Float Settings")]
    public float floatHeight = 0.1f; // How high it moves (adjust as needed)
    public float floatSpeed = 1f;    // How fast it bobs up and down

    private Vector3 startPos;

    private void Start()
    {
        // Store the initial position
        startPos = transform.position;
    }

    private void Update()
    {
        // Calculate a new Y position using Sine wave for smooth oscillation
        float newY = startPos.y + Mathf.Sin(Time.time * floatSpeed) * floatHeight;
        
        // Apply the new position while keeping X and Z the same
        transform.position = new Vector3(startPos.x, newY, startPos.z);
    }
}