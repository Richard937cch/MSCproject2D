using UnityEngine;
using System.Collections;

public class BlockReaction : MonoBehaviour
{
    private Material originalMaterial; // To store the original material
    [Header("TileType")]
    public TileType tiletype;

    [Header("Material")]
    public Material redMaterial; // Assign this in the Inspector
    public Material redMaterial2;
    
    private Renderer objectRenderer;
    /*
    public enum BlockReactionType
    {
        None, Hit, Touch, Restore
    };*/
    
    [Header("Block Reaction Type")]
    public BlockReactionType type;
    //public GameObject changePrefab; //Store prefab for next change

    private int hit = 0;  //how many times block got hit

    [Header("Touch Duration Setup")]

    public float durationTime = 5.0f; // Set the duration time in seconds
    private float elapsedTime = 0f;
    //private float currentDurationTime;
    public bool durationExceeded = false; // Boolean to check if duration is exceeded
    private bool isPlayerTouching = false;
     private Coroutine changeColorCoroutine;

    [Header("Restore Time Setup")]
    public float restoreTime = 5.0f;
    private float restoretimer = 0;
    
    [Header("Change mode")]
    public ChangeType changeType = ChangeType.None;

    

    void Start()
    {
        objectRenderer = GetComponent<Renderer>();
        //currentDurationTime = durationTime;
        restoretimer = restoreTime;
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
        if (type == BlockReactionType.Restore)
        {
            restoreUpdate();
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
                if (changeColorCoroutine != null)
                {
                    StopCoroutine(changeColorCoroutine);
                    changeColorCoroutine = null;
                }
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
                changeType = ChangeType.Block2Back;
            }
            else
            {
                Debug.LogError("Block collison error");
            }
    }

    void durationUpdate()
    {
        if (isPlayerTouching && changeColorCoroutine == null)
        {
            // Start the coroutine to change color gradually
            changeColorCoroutine = StartCoroutine(ChangeColorOverTime());
        }
        /*if (isPlayerTouching && !durationExceeded)
        {
            // Reduce the duration time if the player is touching the object
            currentDurationTime -= Time.deltaTime;

            if (currentDurationTime <= 0)
            {
                // Set the boolean to true if the duration time is exceeded
                durationExceeded = true;
                changeType = ChangeType.Block2Back;
                currentDurationTime = 0; // Ensure the timer doesn't go below zero
                //Debug.Log("Duration time exceeded!");
            }
        }*/
    }

    private IEnumerator ChangeColorOverTime()
    {
        float halfDuration = durationTime / 2.0f;

        while (elapsedTime < durationTime)
        {
            if (isPlayerTouching)
            {
                elapsedTime += Time.deltaTime;

                if (elapsedTime < halfDuration)
                {
                    // Change color from original to redMaterial at first half duration time
                    float t = elapsedTime / halfDuration;
                    Color lerpedColor = Color.Lerp(originalMaterial.color, redMaterial.color, t);
                    objectRenderer.material.color = lerpedColor;
                }
                else
                {
                    // Change color from redMaterial to redMaterial2 at second half duration time
                    float t = (elapsedTime - halfDuration) / halfDuration;
                    Color lerpedColor = Color.Lerp(redMaterial.color, redMaterial2.color, t);
                    objectRenderer.material.color = lerpedColor;
                }
            }
            yield return null;
        }

        // Ensure the final color is set
        objectRenderer.material.color = redMaterial2.color;
        changeType = ChangeType.Block2Back;
    }

    void restoreUpdate()
    {
        restoretimer -= Time.deltaTime;

        if (restoretimer <= 0)
        {
            changeType = ChangeType.Back2Block;
            restoreTime = 0;
        }
    }


}
