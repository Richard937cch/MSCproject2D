using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileManager : MonoBehaviour
{
    public GameObject back;
    public GameObject block;

    private EnumManager enumManager;

    private Gridgen gridgen;
    // Start is called before the first frame update
    void Start()
    {
        gridgen = GetComponent<Gridgen>();
        enumManager = GetComponent<EnumManager>();
        
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
                if (childScript.changeType == ChangeType.Block2Back)
                {
                    ReplaceChildWithPrefab(child,back);
                }
                else if (childScript.changeType == ChangeType.Back2Block)
                {
                    ReplaceChildWithPrefab(child,block);
                    //RestoreChild(child);
                }
                /*else
                {
                    Debug.Log(child.name + " has myBoolValue set to false.");
                }*/

                if (childScript.tiletype != TileType.BackGrounds) //check block reaction type to ensure all in correct block reaction type
                {
                    if (!enumManager.AreTypesEqual(gridgen.blockType, childScript.type))
                    {
                        enumManager.SetBlockType(childScript, gridgen.blockType);
                    }
                }
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
            if (gridgen.backType == BackType.Restore)  //set restore 
            {
                newObject.GetComponent<BlockReaction>().type = BlockReactionType.Restore;
                //newObject.GetComponent<BlockReaction>().changePrefab = child.gameObject; // store old prefab in script for restore
            }
            
            //AstarPath.active.Scan();

            // Destroy the current child game object
            Destroy(child.gameObject);
        }
        else
        {
            Debug.LogError("Please assign a new prefab in the Inspector.");
        }
    }

    /*void RestoreChild(Transform child)
    {
        // Store the current transform values
        Vector3 position = child.position;
        Quaternion rotation = child.rotation;
        Vector3 scale = child.localScale;

        //fetch prefab from pre-store prefab in BlockReaction.changeprefab
        GameObject newPrefab = child.GetComponent<BlockReaction>().changePrefab;

        // Instantiate the new prefab with the same transform values
        GameObject newObject = Instantiate(newPrefab, position, rotation, transform);
        newObject.transform.localScale = scale;

    }*/
}
