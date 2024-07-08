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
    public float respawnDepth;
    private Coroutine damageCoroutine;

    private bool isInvincible = false;
    public float invincibilityDuration = 10;

    public Material normalMaterial;
    public Material invincibleMaterial;
    private Renderer playerRenderer;
    
    GM gm;

    Gridgen gridgen;

    private Rigidbody2D rb;

    public MapSettings mapSettings;

    void Start()
    {
        gridgen = GameObject.Find("MapGenerator").GetComponent<Gridgen>();
        gm = GameObject.Find("GameController").GetComponent<GM>();
        playerRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        //GameObject healthTextObject = GameObject.Find("HP");
        //healthText = healthTextObject.GetComponent<TextMeshProUGUI>();
        MenuParameter(); //receive setup from main manu
        currentHealth = maxHealth;
        gm.UpdateHPtext(currentHealth);
        gm.UpdateLifetext(Life);
        //respawnDepth = (gridgen.height/2+5)* -1;
        //respawnDepth = (Mathf.Max(gridgen.width, gridgen.height) / 2 + 5) * -1;
        respawnDepth = Mathf.Sqrt(Mathf.Pow(gridgen.width, 2) + Mathf.Pow(gridgen.height, 2)) * -1 - 5;

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
            // Reduce health
            if (!isInvincible)
            {
                TakeDamage(10);
            }
            
        }
        
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Token") )//&& collision.gameObject.GetComponent<Token>().tokenType == TokenType.Invincible)
        {
            GameObject token = collision.gameObject;
            TokenType type = collision.gameObject.GetComponent<Token>().tokenType;
            if (type == TokenType.Score)           //score token
            {
                gm.TokenCollected();
                gm.UpdateTokenCountText();
            }
            else if (type == TokenType.Invincible) //invincible token
            {
                StartCoroutine(ActivateInvincibility());
            }
            Destroy(token);
            
            
        }

        if (collision.CompareTag("Lava"))// Take damage when collide with lava
        {
            TakeDamage(10);
        }
    }

    private void OnTriggerStay2D(Collider2D collision) // Take damage every second in the lava
    {
        if (collision.CompareTag("Lava"))
        {
            if (damageCoroutine == null)
            {
                damageCoroutine = StartCoroutine(DamageOverTime(1f));
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision) // stop taking damage when leave lava
    {
        if (collision.CompareTag("Lava"))
        {
            if (damageCoroutine != null)
            {
                StopCoroutine(damageCoroutine);
                damageCoroutine = null;
            }
        }
    }

    private IEnumerator DamageOverTime(float time)
    {
        while (true)
        {
            yield return new WaitForSeconds(time);
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

    private IEnumerator ActivateInvincibility()
    {
        isInvincible = true;
        playerRenderer.material = invincibleMaterial;
        yield return new WaitForSeconds(invincibilityDuration);
        isInvincible = false;
        playerRenderer.material = normalMaterial;
    }

    void MenuParameter() //if not using editor setup, use setup from main menu
    {
        if (!gridgen.editorValue)
        {
            
            Life = mapSettings.life;
            maxHealth = mapSettings.hp;
        }
        
    }

    void respawn()
    {
		transform.position = gridgen.spawnpoint; // back to respawnpoint
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
