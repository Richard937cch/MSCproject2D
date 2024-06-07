using UnityEngine;

public class BlockReaction : MonoBehaviour
{
    private Material originalMaterial; // To store the original material

    public Material redMaterial; // Assign this in the Inspector
    
    private Renderer objectRenderer;

    void Start()
    {
        objectRenderer = GetComponent<Renderer>();

        if (objectRenderer != null)
        {
            originalMaterial = objectRenderer.material; // Store the original material
        }
        else
        {
            Debug.LogError("No Renderer found on the object. Please add a Renderer component.");
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (objectRenderer != null && redMaterial != null)
            {
                objectRenderer.material = redMaterial;
            }
            else
            {
                Debug.LogError("Please assign the red material in the Inspector.");
            }
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (objectRenderer != null)
            {
                objectRenderer.material = originalMaterial; // Restore the original material
            }
        }
    }
}
