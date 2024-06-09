using UnityEngine;

public class BlockReaction : MonoBehaviour
{
    private Material originalMaterial; // To store the original material

    [Header("Material")]
    public Material redMaterial; // Assign this in the Inspector
    public Material redMaterial2;
    
    private Renderer objectRenderer;
    
    public enum BlockReactionType
    {
        Hit, Touch
    };
    
    [Header("Block Reaction Type")]
    public BlockReactionType type;

    private int hit = 0;  //how many times block got hit

    [Header("Touch Duration Setup")]

    public float durationTime = 5.0f; // Set the duration time in seconds
    private float currentDurationTime;
    public bool durationExceeded = false; // Boolean to check if duration is exceeded

    private bool isPlayerTouching = false;

    [Header("Change mode")]
    public int change = 0;

    

    void Start()
    {
        objectRenderer = GetComponent<Renderer>();
        currentDurationTime = durationTime;

        if (objectRenderer != null)
        {
            originalMaterial = objectRenderer.material; // Store the original material
        }
        else
        {
            Debug.LogError("No Renderer found on the object. Please add a Renderer component.");
        }
    }

    void Update()
    {
        if (type == BlockReactionType.Touch)
        {
            durationUpdate();
        }
        
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (type == BlockReactionType.Hit) //hit
            {
                GetHit();
            }            
            else if (type == BlockReactionType.Touch) //touch
            {
                isPlayerTouching = true;
            }
            
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (type == BlockReactionType.Touch)
            {
                isPlayerTouching = false;
            }
            /*if (objectRenderer != null)
            {
                objectRenderer.material = originalMaterial; // Restore the original material
            }*/
        }
    }

    void GetHit()
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

    void durationUpdate()
    {
        if (isPlayerTouching && !durationExceeded)
        {
            // Reduce the duration time if the player is touching the object
            currentDurationTime -= Time.deltaTime;

            if (currentDurationTime <= 0)
            {
                // Set the boolean to true if the duration time is exceeded
                durationExceeded = true;
                change = 1;
                currentDurationTime = 0; // Ensure the timer doesn't go below zero
                //Debug.Log("Duration time exceeded!");
            }
        }
    }


}
