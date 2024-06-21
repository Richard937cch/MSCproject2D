using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Token : MonoBehaviour
{
    public string playerTag = "Player"; // Tag used to identify the player
    private GM gm;

    void Start ()
    {
        gm = GameObject.Find("GameController").GetComponent<GM>();
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(playerTag))
        {
            gm.TokenCollected();
            gm.UpdateTokenCountText();
            Destroy(gameObject);
        }
    }
}
