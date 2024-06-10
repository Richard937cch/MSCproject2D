using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class PlayerState : MonoBehaviour
{
    public int maxHealth = 100;
    private int currentHealth;

    public TextMeshProUGUI healthText; // Reference to the UI Text element

    void Start()
    {
        GameObject healthTextObject = GameObject.Find("HP");
        healthText = healthTextObject.GetComponent<TextMeshProUGUI>();
        currentHealth = maxHealth;
        UpdateHealthText();
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
        UpdateHealthText();

        // Check if health is zero or less
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void UpdateHealthText()
    {
        // Update the health text on the UI
        if (healthText != null)
        {
            healthText.text = "HP: " + currentHealth;
        }
    }

    void Die()
    {
        // Handle player death
        Debug.Log("Player Died");
        // You can add additional logic here, such as playing a death animation, restarting the game, etc.
    }
}
