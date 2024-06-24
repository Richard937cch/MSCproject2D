using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class PlayerState : MonoBehaviour
{
    public int Life = 3;
    public int maxHealth = 100;
    private int currentHealth;
    public float respawnDepth = -110;

    
    GM gm;

    void Start()
    {
        gm = GameObject.Find("GameController").GetComponent<GM>();
        //GameObject healthTextObject = GameObject.Find("HP");
        //healthText = healthTextObject.GetComponent<TextMeshProUGUI>();
        currentHealth = maxHealth;
        gm.UpdateHPtext(currentHealth);
        gm.UpdateLifetext(Life);
    }

    void Update()
    {
        if (transform.position.y < respawnDepth)
		{
            respawn();
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // Check if the player is hit by an enemy
        if (collision.gameObject.CompareTag("Enemy"))
        {
            print("000");
            // Reduce health
            TakeDamage(10);
        }
    }

    void TakeDamage(int damage)
    {
        currentHealth -= damage;
        //Debug.Log("Player Health: " + currentHealth);
        gm.UpdateHPtext(currentHealth);

        // Check if health is zero or less
        if (currentHealth <= 0)
        {
            respawn();
            
        }
    }

    

    void respawn()
    {
		transform.position = new Vector3(0, 40, -0.58f); // back to respawnpoint
        Life--;                                          //lose one life
        gm.UpdateLifetext(Life);
        if (Life == 0) //if run out of life, die
        {
            Die();
        }

        currentHealth = maxHealth;   // hp back to full
        gm.UpdateHPtext(currentHealth);
    }

    void Die()
    {
        // Handle player death
        //Debug.Log("Player Died");
        // You can add additional logic here, such as playing a death animation, restarting the game, etc.
        gm.Lose(); // Lose if ran out of life
    }
}
