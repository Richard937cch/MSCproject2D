using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileManager : MonoBehaviour
{
    public GameObject back;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        changeCheck();
    }

    void changeCheck()
    {
        foreach (Transform child in transform)
        {
            
            // Get the ChildScript component from the child
            BlockReaction childScript = child.GetComponent<BlockReaction>();

            if (childScript != null)
            {
                // Check the boolean value
                if (childScript.change == 1)
                {
                    ReplaceChildWithPrefab(child,back);
                }
                /*else
                {
                    Debug.Log(child.name + " has myBoolValue set to false.");
                }*/
            }
            /*else
            {
                Debug.LogWarning(child.name + " does not have a ChildScript component.");
            }*/
        }
    }
    void ReplaceChildWithPrefab(Transform child, GameObject newPrefab)
    {
        if (newPrefab != null)
        {
            // Store the current transform values
            Vector3 position = child.position;
            Quaternion rotation = child.rotation;
            Vector3 scale = child.localScale;

            // Instantiate the new prefab with the same transform values
            GameObject newObject = Instantiate(newPrefab, position, rotation, transform);
            newObject.transform.localScale = scale;

            // Destroy the current child game object
            Destroy(child.gameObject);
        }
        else
        {
            Debug.LogError("Please assign a new prefab in the Inspector.");
        }
    }
}
