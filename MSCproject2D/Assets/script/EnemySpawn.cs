using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawn : MonoBehaviour
{
    public GameObject enemyPrefab; // Reference to the enemy prefab
    public float spawnRange = 5f; // Range within which to spawn enemies
    public float spawnInterval = 20f; // Time interval between spawns

    public float duration = 10f;

    private GameObject player; // Reference to the player GameObject
    private float timer;

    void Start()
    {
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
        
        if (timer <= 0.0f)
        {
            SpawnEnemyNearPlayer();
            timer = spawnInterval; // Reset the timer
        }

        if (GameObject.FindGameObjectWithTag("Player") == null)
        {
            print("survived!");
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
        spawnPosition.z = 0.2f;

        // Instantiate the enemy at the calculated position
        Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);

        Debug.Log("Spawned enemy at: " + spawnPosition);
    }
}
