using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;


public class GM : MonoBehaviour
{
    public TextMeshProUGUI healthText; 
    public TextMeshProUGUI LifeText; 

    public TextMeshProUGUI tokenText; 
    private int tokenCount = 0; // Variable to keep track of token count
    private int totalTokens = 0;

    public Button PauseButton;
    [SerializeField] GameObject Pausemenu;

    private bool isPaused = false;

    //public int Life = 3;

    [SerializeField] GameObject Winmenu;
    [SerializeField] GameObject Losemenu;
    

    // Start is called before the first frame update
    void Start()
    {
        // Count the initial number of tokens in the scene
        CountInitialTokens();
        // Initialize the token count text
        UpdateTokenCountText();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateHPtext(int currentHealth)
    {
        healthText.text = "HP: " + currentHealth;
    }

    public void UpdateLifetext(int currentLife)
    {
        LifeText.text = "Life: " + currentLife;
    }

    public void UpdateTokenCountText()
    { 
        tokenText.text = "Tokens: " + tokenCount.ToString()+" / "+ totalTokens;
    }

    public void TokenCollected()
    {
        tokenCount++;
        if (tokenCount == totalTokens) //Win if collect all token
        {
            Win();
        }
    }
     void CountInitialTokens()
    {
        // Find all GameObjects with the tag "Token"
        GameObject[] tokens = GameObject.FindGameObjectsWithTag("Token");
        totalTokens = tokens.Length;
    }

    public void Pause()
    {
        if (!isPaused)
        {
            Pausemenu.SetActive(true);
            Time.timeScale = 0;
            PauseButton.GetComponentInChildren<TextMeshProUGUI>().text = "Resume";  
            isPaused = true;
        }
        else
        {
            Resume();
        }
        
    }

    public  void Resume()
    {
        Pausemenu.SetActive(false);
        Time.timeScale = 1;
        PauseButton.GetComponentInChildren<TextMeshProUGUI>().text = "Pause";
        isPaused = false;
    }

    public void Lose()
    {
        Time.timeScale = 0;
        Losemenu.SetActive(true);
        PauseButton.gameObject.SetActive(false);
    }

    public void Win()
    {
        Time.timeScale = 0;
        Winmenu.SetActive(true);
        PauseButton.gameObject.SetActive(false);
    }

    public void Restart()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        string sceneName = currentScene.name;
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
        Time.timeScale = 1;
    }

}
