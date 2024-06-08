using UnityEngine;

public class BlockReaction : MonoBehaviour
{
    private Material originalMaterial; // To store the original material

    public Material redMaterial; // Assign this in the Inspector
    public Material redMaterial2;
    
    private Renderer objectRenderer;

    //public GameObject newPrefab;

    private int hit = 0;

    public int change = 0;

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
            
            
            if (objectRenderer != null && redMaterial != null && hit == 0)
            {
                objectRenderer.material = redMaterial;
                hit++;
            }
            else if (hit == 1 && redMaterial2 != null)
            {
                objectRenderer.material = redMaterial2;
                hit++;
            }
            else if (hit >=2) 
            {
                change = 1;
            }
            else
            {
                Debug.LogError("Block collison error");
            }
        }
    }
/*
    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (objectRenderer != null)
            {
                objectRenderer.material = originalMaterial; // Restore the original material
            }
        }
    }*/
/*    public void Replace()
    {
        if (newPrefab != null)
        {
            // Store the current transform values
            Vector3 position = transform.position;
            Quaternion rotation = transform.rotation;
            Vector3 scale = transform.localScale;

            // Instantiate the new prefab with the same transform values
            GameObject newObject = Instantiate(newPrefab, position, rotation);
            newObject.transform.localScale = scale;

            // Destroy the current game object
            Destroy(gameObject);
        }
        else
        {
            Debug.LogError("Please assign a new prefab in the Inspector.");
        }
    }*/
}
