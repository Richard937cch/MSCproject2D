using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileManager : MonoBehaviour
{
    public GameObject back;
    public GameObject block;
    private GameObject player;

    private EnumManager enumManager;

    private Gridgen gridgen;
    private AStarGridControl astarGridControl;

    public bool shrink = false;
    
    public float cullDistance = 25.0f; // Distance within which tiles are active
    public float enemycullDistance = 10.0f; // Distance within which tiles are active
    private List<Transform> enemies = new List<Transform>(); // List of enemies
    void Start()
    {
        gridgen = GetComponent<Gridgen>();
        enumManager = GetComponent<EnumManager>();
        astarGridControl = GameObject.Find("A*").GetComponent<AStarGridControl>();
        player = GameObject.FindGameObjectWithTag("Player");
        StartCoroutine(UpdateTileVisibility());
    }

    // Update is called once per frame
    void Update()
    {
        changeCheck();
    }
    IEnumerator UpdateTileVisibility() //culling manager
    {
        while (true)
        {
            Vector3 playerPosition = player.transform.position;
            bool isPlayerOrEnemyInRange;
            UpdateEnemyList();

            Vector3 respawnp = gridgen.spawnpoint;
            // Activate tiles within the cullDistance of player or enemies
            foreach (Transform tile in transform)
            {
                Vector3 tilePosition = tile.transform.position;
                float distanceToPlayer = Vector3.Distance(playerPosition, tilePosition);
                float distanceToSpawn = Vector3.Distance(respawnp, tilePosition);
                // Check if any enemy is within the cullDistance
                bool isEnemyInRange = false;
                foreach (Transform enemy in enemies)
                {
                    if (Vector3.Distance(enemy.position, tilePosition) < enemycullDistance)
                    {
                        isEnemyInRange = true;
                        break;
                    }
                }

                // Check if the tile is within the cullDistance of player or any enemy
                isPlayerOrEnemyInRange = distanceToPlayer < cullDistance || isEnemyInRange || distanceToSpawn < cullDistance;

                
                // Activate or deactivate the tile based on proximity and visibility
                tile.gameObject.SetActive(isPlayerOrEnemyInRange ||
                tile.gameObject.tag == "Slime" || tile.gameObject.tag == "Restore"
                );
            }

            yield return new WaitForSeconds(0.1f);
        }
    }

    void UpdateEnemyList()
    {
        enemies.Clear();
        GameObject[] enemyObjects = GameObject.FindGameObjectsWithTag("Enemy");

        foreach (GameObject enemyObject in enemyObjects)
        {
            enemies.Add(enemyObject.transform);
        }
    }

    /*bool TileActive(Transform tile, Vector3 playerPosition)
    {
        
        Vector3 tilePosition = tile.transform.position;
        float distanceToPlayer = Vector3.Distance(playerPosition, tilePosition);
        if (distanceToPlayer < cullDistance || 
            tile.gameObject.tag == "Slime" || 
            tile.GetComponent<BlockReaction>().type == BlockReactionType.Restore)
        {
            return true;
        }
        return false;

    }*/

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
                    ReplaceChildWithPrefab(child, back, 0.1f);
                }
                else if (childScript.changeType == ChangeType.Back2Block)
                {
                    ReplaceChildWithPrefab(child, block, -0.2f);
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
    void ReplaceChildWithPrefab(Transform child, GameObject newPrefab, float depth)
    {
        if (newPrefab != null)
        {
            // Store the current transform values
            Vector3 position = child.position;
            Quaternion rotation = child.rotation;
            Vector3 scale = child.localScale;
            //set depth for block(-0.1) and back(0.1) for hiden token adaption
            position.z = depth;
            
            // Instantiate the new prefab with the same transform values
            GameObject newObject = Instantiate(newPrefab, position, rotation, transform);
            newObject.transform.localScale = scale;
            BlockReaction newBR = newObject.GetComponent<BlockReaction>();
            if (gridgen.backType == BackType.Restore)  //set restore 
            {
                newBR.type = BlockReactionType.Restore;
                //newObject.GetComponent<BlockReaction>().changePrefab = child.gameObject; // store old prefab in script for restore
                if (newBR.tiletype == TileType.BackGrounds)
                {
                    newObject.tag = "Restore";
                }
            }
            //update node
            //print(newObject.transform.localPosition);
            Vector3Int newnode = Vector3Int.FloorToInt(newObject.transform.localPosition);
            //newnode -= new Vector3Int (gridgen.width/2, gridgen.height/2, 0);
            astarGridControl.UpdateNodeWalkability(newnode);
            //StartCoroutine(Rescan());
            
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
