using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LavaMovement : MonoBehaviour
{
    private float minY; // Minimum height of the lava
    private float maxY; // Maximum height of the lava
    public float speed = 1f; // Speed of the lava's movement

    private bool movingUp = true;
    Gridgen gridgen;

    void Start ()
    {
        gridgen = GameObject.Find("MapGenerator").GetComponent<Gridgen>();
        minY = this.transform.position.y - gridgen.height/2;
        maxY = this.transform.position.y + gridgen.height/2 - 3;
    }
    void Update()
    {
        // Calculate the new position of the lava
        float newY = transform.position.y + (movingUp ? speed * Time.deltaTime : -speed * Time.deltaTime);

        // Check if the lava has reached the max or min height
        if (newY >= maxY)
        {
            newY = maxY;
            movingUp = false;
        }
        else if (newY <= minY)
        {
            newY = minY;
            movingUp = true;
        }

        // Apply the new position
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }
}
