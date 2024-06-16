using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class EnemyAStar : MonoBehaviour
{
    //ensure you have the 'using Pathfinding;' above the script to get pathfinding components to work. 

    //Insert component you want enemy to folow in editor
    public Transform target;

    //The speed of your enemy
    public float speed = 200f;
    //individual gridpoints which your enemy moves to to get to the player
    public float nextWaypointDistance = 3f;

    //Enemy Sprite, Disregard if you dont want the sprite to flip to look at the player
    //public Transform enemyGFX; 

    Path path;
    int currentWayPoint;
    bool reachedEndOfPath;

    Seeker seeker;
    Rigidbody2D rb;

    void Start()
    {
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();
        target = GameObject.FindGameObjectWithTag("Player").transform;
        InvokeRepeating("UpdatePath", 0f, 0.5f); 
        
    }

    void UpdatePath()
    {
        if (seeker.IsDone())
        {
            seeker.StartPath(rb.position, target.position, OnPathComplete);
        }

    }

    void OnPathComplete(Path p)
    {
        if(!p.error)
        {
            path = p;
            currentWayPoint = 0; 
        }
    }

    void FixedUpdate()
    {
        if (path == null)
            return; 

        if(currentWayPoint >= path.vectorPath.Count)
        {
            reachedEndOfPath = true;
            return; 
        } else
        {
            reachedEndOfPath = false;
        }

        Vector2 direction = ((Vector2)path.vectorPath[currentWayPoint] - rb.position).normalized; 
        //Vector2 force = direction * speed * Time.deltaTime;
        Vector2 force = direction * speed * Time.fixedDeltaTime;

        rb.AddForce(force);

        float distane = Vector2.Distance(rb.position, path.vectorPath[currentWayPoint]);

        
        if (distane < nextWaypointDistance) 
        {
            currentWayPoint++;
        }

        //Disregard below if you dont want the sprite to flip to look at the player.
        //You can change 'force' to 'rb.velocity' if you want the sprite to flip depending on the velocity and not the direction it is travelling toward the player.
        /*if (force.x >= 0.01f)
        {
            enemyGFX.localScale = new Vector3(-1f, 1f, 1f);
        }
        else if (force.x <= -0.01f)
        {
            enemyGFX.localScale = new Vector3(1f, 1f, 1f);
        }*/
    }
}