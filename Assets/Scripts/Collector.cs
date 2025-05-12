using UnityEngine;

public class Collector : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        iItem item = collision.GetComponent<iItem>();
        if (item != null)
        {
            item.Collect();
        }
    }
}
