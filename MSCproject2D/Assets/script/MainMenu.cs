using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;


public class MainMenu : MonoBehaviour
{
    public Scrollbar widthInput;
    public Scrollbar heightInput;
    public TMP_InputField seedInput;
    public Scrollbar scoreTokenInput;
    public Scrollbar perkTokenInput;
    public TMP_Dropdown mapTypeDropdown;
    public TMP_Dropdown blockTypeDropdown;
    public TMP_Dropdown backTypeDropdown;
    public Scrollbar rotationSpeedInput;
    public Scrollbar jumpInput;
    public Scrollbar LifeInput;
    public Scrollbar HPInput;
    public MapSettings mapSettings;


    private float w;
    private float h;
    private string s;
    private float sc;
    private float p;
    private int map;
    private int block;
    private int back;
    private float r;
    private float l;
    private float hp;


    public void Start()
    {
        initSetup();
    }

    public void Update()
    {
        setMapValue();
        //print(widthInput.value);
    }

    public void StartGame()
    {
        SceneManager.LoadScene("GameScene");
    }

    private void setMapValue()
    {
        //set Value
        mapSettings.width = Convert.ToInt32(widthInput.value*40+5)*2;
        mapSettings.height = Convert.ToInt32(heightInput.value*40+5)*2;
        mapSettings.scoreTokenAmount = Convert.ToInt32(scoreTokenInput.value*100+1);
        mapSettings.perkTokenAmount = Convert.ToInt32(perkTokenInput.value*100+1);
        mapSettings.rotationSpeed = Convert.ToInt32(rotationSpeedInput.value*100+1);
        mapSettings.jump = Convert.ToInt32(jumpInput.value*30+1);
        mapSettings.life = Convert.ToInt32(LifeInput.value*100+1);
        mapSettings.hp = Convert.ToInt32(HPInput.value*100+1)*10;
        mapSettings.mapType = (MapType)mapTypeDropdown.value;
        mapSettings.blockType = (BlockType)blockTypeDropdown.value;
        mapSettings.backType = (BackType)backTypeDropdown.value;
        mapSettings.seed = int.Parse(seedInput.text);

        //set Text
        widthInput.GetComponentInChildren<TextMeshProUGUI>().text = "Width: " + mapSettings.width;
        heightInput.GetComponentInChildren<TextMeshProUGUI>().text = "Height: " + mapSettings.height;
        scoreTokenInput.GetComponentInChildren<TextMeshProUGUI>().text = "Score Token: " + mapSettings.scoreTokenAmount;
        perkTokenInput.GetComponentInChildren<TextMeshProUGUI>().text = "Perk Token: " + mapSettings.perkTokenAmount; 
        rotationSpeedInput.GetComponentInChildren<TextMeshProUGUI>().text = "Rotation speed: " + mapSettings.rotationSpeed;
        jumpInput.GetComponentInChildren<TextMeshProUGUI>().text = "Jump height: " + mapSettings.jump;
        LifeInput.GetComponentInChildren<TextMeshProUGUI>().text = "Life: " + mapSettings.life;
        HPInput.GetComponentInChildren<TextMeshProUGUI>().text = "HP: " + mapSettings.hp;

        //save UI value
        mapSettings.w = widthInput.value;
        mapSettings.h = heightInput.value;
        mapSettings.sc = scoreTokenInput.value;
        mapSettings.p = perkTokenInput.value;
        mapSettings.r = rotationSpeedInput.value;
        mapSettings.j = jumpInput.value;
        mapSettings.l = LifeInput.value;
        mapSettings.hpi = HPInput.value;
        mapSettings.map = mapTypeDropdown.value;
        mapSettings.block = blockTypeDropdown.value;
        mapSettings.back = backTypeDropdown.value;
        mapSettings.s = seedInput.text;
    }

    void initSetup()
    {
        
        
        widthInput.value = mapSettings.w;
        heightInput.value = mapSettings.h;
        scoreTokenInput.value = mapSettings.sc;
        perkTokenInput.value = mapSettings.p;
        rotationSpeedInput.value = mapSettings.r;
        jumpInput.value = mapSettings.j;
        LifeInput.value = mapSettings.l;
        HPInput.value = mapSettings.hpi;
        mapTypeDropdown.value = mapSettings.map;
        blockTypeDropdown.value = mapSettings.block;
        backTypeDropdown.value= mapSettings.back;
        seedInput.text = mapSettings.s;
        
    }

    
}
