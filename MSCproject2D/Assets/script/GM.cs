using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GM : MonoBehaviour
{
    public TextMeshProUGUI tokenText; // Reference to the UI Text element
    private int tokenCount = 0; // Variable to keep track of token count
    private int totalTokens = 0;

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

    public void UpdateTokenCountText()
    { 
        tokenText.text = "Tokens: " + tokenCount.ToString()+" / "+ totalTokens;
    }

    public void TokenCollected()
    {
        tokenCount++;
    }
     void CountInitialTokens()
    {
        // Find all GameObjects with the tag "Token"
        GameObject[] tokens = GameObject.FindGameObjectsWithTag("Token");
        totalTokens = tokens.Length;
    }
}
