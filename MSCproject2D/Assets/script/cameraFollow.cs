using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraFollow : MonoBehaviour
{
    public GameObject player;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }
    void LateUpdate()
    {
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }
        Vector3 desiredPosition = player.transform.position + new Vector3(0f, 0f, -20f);
        transform.position = desiredPosition;
        transform.LookAt(player.transform);
    }
}
