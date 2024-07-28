using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class EnemySpawn : MonoBehaviour
{
    public bool EnemyEnable = true;
    public GameObject enemyPrefab; // Reference to the enemy prefab
    public float spawnRange = 5f; // Range within which to spawn enemies
    public float spawnInterval = 20f; // Time interval between spawns

    public float duration = 10f;

    private GameObject player; // Reference to the player GameObject
    private float timer;

    private GridGraph gridGraph;
    private Gridgen gridgen;
    public MapSettings mapSettings;

    void Start()
    {
        gridGraph = AstarPath.active.data.gridGraph;
        gridgen = GameObject.Find("MapGenerator").GetComponent<Gridgen>();
        MenuParameter(); //receive setup from main manu
        timer = spawnInterval; // Initialize the timer
        if (enemyPrefab == null)
        {
            Debug.LogError("Please assign the enemy prefab in the inspector.");
            return;
        }
        
        player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            Debug.LogError("Player not found. Make sure the player has the 'Player' tag.");
            return;
        }
        
        
        
    }

    void Update()
    {
        timer -= Time.deltaTime;
        
        if (timer <= 0.0f && EnemyEnable) //spawn enemy
        {
            FindWalkablePositionNearPlayer();
            timer = spawnInterval; // Reset the timer
        }

        if (GameObject.FindGameObjectWithTag("Player") == null)
        {
            print("null player");
        }
    }

    void SpawnEnemyNearPlayer()
    {
        print("enemy");
        // Calculate a random position within the spawn range around the player
        Vector3 randomOffset = new Vector3(
            Random.Range(-spawnRange, spawnRange),
            
            0f, // Assuming a 2D game; set this to Random.Range(-spawnRange, spawnRange) for 3D
            Random.Range(-spawnRange, spawnRange)
        );

        //Vector3 spawnPosition = player.transform.position + randomOffset;
        Vector3 spawnPosition = GameObject.FindGameObjectWithTag("Player").transform.position;

        spawnPosition+=randomOffset;
        spawnPosition.z = -0.3f;

        // Instantiate the enemy at the calculated position
        Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);

        Debug.Log("Spawned enemy at: " + spawnPosition);
    }

    void FindWalkablePositionNearPlayer()
    {
        List<GridNode> walkableNodes = new List<GridNode>();
        Vector3 playerPosition = player.transform.position;

        // Iterate through all nodes in the grid
        for (int x = 0; x < gridGraph.width; x++)
        {
            for (int z = 0; z < gridGraph.depth; z++)
            {
                GridNode node = gridGraph.GetNode(x, z) as GridNode;

                if (node != null && node.Walkable)
                {
                    // Convert the node position to world position
                    Vector3 nodeWorldPosition = (Vector3)node.position;

                    // Check if the node is within the spawn radius around the player
                    if (Vector3.Distance(playerPosition, nodeWorldPosition) <= spawnRange)
                    {
                        walkableNodes.Add(node);
                    }
                }
            }
        }

        if (walkableNodes.Count > 0)
        {
            // Select a random walkable node
            GridNode randomNode = walkableNodes[Random.Range(0, walkableNodes.Count)];

            // Convert the node position to world position
            Vector3 worldPosition = (Vector3)randomNode.position;
            //return worldPosition;
            GameObject newEnemy = Instantiate(enemyPrefab, worldPosition, Quaternion.identity);
            newEnemy.transform.parent = transform;
        }
        else
        {
            print("no enemy spawnpoint");
        }

    }
    void MenuParameter() //if not using editor setup, use setup from main menu
    {
        if (!gridgen.editorValue)
        {
            EnemyEnable = mapSettings.enemyEnable;
        }
        
    }
}
