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

    public float jumpHeight = 5.0F;

    public bool isFalling = false;

    public float duration = 10f; // Time interval between spawns
    private float timer;
 

    Path path;
    int currentWayPoint;
    bool reachedEndOfPath;

    Seeker seeker;
    Rigidbody2D rb;

    public bool isInSlime = false;
    public bool isInLava = false;
    public float slimeSpeed = 0.9f;
    private Coroutine deformCoroutine;
    private DeformableSlimeBlock slimeDeform;


    void Start()
    {
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();
        target = GameObject.FindGameObjectWithTag("Player").transform;
        //isFalling = true;
        InvokeRepeating("UpdatePath", 0f, 0.5f); 
        timer = duration;
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

    void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0.0f)
        {
            Destroy(gameObject);
            timer = duration; // Reset the timer
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
		
        //jump
        float height =  rb.position.y - path.vectorPath[currentWayPoint].y;
        //Vector2 jumpdirection = new Vector2(0,direction.y);
        //Vector2 jumpforce = jumpdirection * speed * Time.fixedDeltaTime; 
        if (height < 0 && !isFalling)
        {
            //print(height);
            rb.AddForce (Vector3.up * jumpHeight, (ForceMode2D)ForceMode.Impulse);
        }

        if (isInSlime || isInLava)
		{
			ModifySpeed(slimeSpeed);
		}

        if (isInSlime)
		{
			this.transform.SetParent(GameObject.Find("MapGenerator").transform);
		}

		if (!isInSlime)
        {
            this.transform.SetParent(GameObject.Find("GameController").transform);
        }

        //reach waypoint or not
        float distane = Vector2.Distance(rb.position, path.vectorPath[currentWayPoint]);
        if (distane < nextWaypointDistance) 
        {
            currentWayPoint++;
        }
    }

    public void OnCollisionStay2D (Collision2D col) 
	{ //Takes parameter of Collision so unity doesn't complain
		isFalling = false;
	}

	public void OnCollisionExit2D() 
	{
		isFalling = true;
	}

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Slime"))
        {
            isInSlime = true;
        }
		if (other.CompareTag("Lava"))
        {
            isInLava = true;
        }
        /*if (other.CompareTag("Slime") || other.CompareTag("Lava"))
        {
            isInSlime = true;
            slimeDeform = other.GetComponent<DeformableSlimeBlock>();
            if (slimeDeform != null)
            {
                deformCoroutine = StartCoroutine(slimeDeform.DeformEdgePoints(transform, "e"));
            }
        }*/
    }

	void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Slime"))
        {
            isInSlime = false;
        }
		if (other.CompareTag("Lava"))
        {
            isInLava = false;
        }
        /*if (other.CompareTag("Slime") || other.CompareTag("Lava"))
        {
            isInSlime = false;
            if (slimeDeform != null && deformCoroutine != null)
            {
                StopCoroutine(deformCoroutine);
                //StartCoroutine(slimeDeform.RestoreEdgePoints());
                deformCoroutine = null;
            }
        }*/
    }

    public void ModifySpeed(float factor)
    {
        rb.velocity *= factor;
    }
}