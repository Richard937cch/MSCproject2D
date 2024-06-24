using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Token : MonoBehaviour
{
    public string playerTag = "Player"; // Tag used to identify the player
    private GM gm;

    public TokenType tokenType;

    void Start ()
    {
        //gm = GameObject.Find("GameController").GetComponent<GM>();
    }
    /*void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(playerTag))
        {
            if (tokenType == TokenType.Score)
            {
                gm.TokenCollected();
                gm.UpdateTokenCountText();
            }
            
            Destroy(gameObject);
        }
    }*/
}
